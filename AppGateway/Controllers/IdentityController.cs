using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using AppGateway.Models;
using AppGateway.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Persistence.Entities;

namespace AppGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender<User> _emailSender;
        private readonly JwtHandler _jwtHandler;
        private readonly IOptionsMonitor<BearerTokenOptions> _bearerTokenOptions;
        private readonly TimeProvider _timeProvider;
        private static readonly EmailAddressAttribute _emailAddressAttribute = new();

        public IdentityController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender<User> emailSender,
            IOptionsMonitor<BearerTokenOptions> bearerTokenOptions,
            JwtHandler jwtHandler,
            TimeProvider timeProvider)
        {
            _jwtHandler = jwtHandler;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _bearerTokenOptions = bearerTokenOptions;
            _timeProvider = timeProvider;
        }

        [HttpPost("addEmployee")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registration, [FromQuery] string InputRole)
        {
            if (!Enum.TryParse<Roles>(InputRole.ToUpper(), out var role))
            {
                return ValidationProblem("Invalid role.");
            }

            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Requires a user store with email support.");
            }

            var email = registration.Email;
            if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
            {
                return ValidationProblem("Invalid email.");
            }

            var user = new User();
            await _userManager.SetUserNameAsync(user, email);
            await _userManager.SetEmailAsync(user, email);

            var result = await _userManager.CreateAsync(user, registration.Password);
            if (!result.Succeeded)
            {
                return ValidationProblem(result.Errors.First().Description);
            }

            result = await _userManager.AddToRoleAsync(user, role.ToString());

            if (!result.Succeeded)
            {
                return ValidationProblem(result.Errors.First().Description);
            }

            await SendConfirmationEmailAsync(user, email);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login, [FromQuery] bool? useCookies,
            [FromQuery] bool? useSessionCookies)
        {
            var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
            var isPersistent = (useCookies == true) && (useSessionCookies != true);
            _signInManager.AuthenticationScheme =
                useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

            var result =
                await _signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent,
                    lockoutOnFailure: true);

            if (result.RequiresTwoFactor)
            {
                if (!string.IsNullOrEmpty(login.TwoFactorCode))
                {
                    result = await _signInManager.TwoFactorAuthenticatorSignInAsync(login.TwoFactorCode, isPersistent,
                        rememberClient: isPersistent);
                }
                else if (!string.IsNullOrEmpty(login.TwoFactorRecoveryCode))
                {
                    result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(login.TwoFactorRecoveryCode);
                }
            }

            if (!result.Succeeded)
            {
                return Unauthorized(result.ToString());
            }

            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
        {
            var refreshTokenProtector = _bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
            var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

            if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
                _timeProvider.GetUtcNow() >= expiresUtc ||
                await _signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not User user)
            {
                return Challenge();
            }

            var newPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            return SignIn(newPrincipal, IdentityConstants.BearerScheme);
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string code,
            [FromQuery] string? changedEmail)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException)
            {
                return Unauthorized();
            }

            IdentityResult result;
            if (string.IsNullOrEmpty(changedEmail))
            {
                result = await _userManager.ConfirmEmailAsync(user, code);
            }
            else
            {
                result = await _userManager.ChangeEmailAsync(user, changedEmail, code);
                if (result.Succeeded)
                {
                    result = await _userManager.SetUserNameAsync(user, changedEmail);
                }
            }

            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            return Content("Thank you for confirming your email.");
        }

        [HttpPost("resendConfirmationEmail")]
        public async Task<IActionResult> ResendConfirmationEmail(
            [FromBody] ResendConfirmationEmailRequest resendRequest)
        {
            var user = await _userManager.FindByEmailAsync(resendRequest.Email);
            if (user == null)
            {
                return Ok();
            }

            await SendConfirmationEmailAsync(user, resendRequest.Email);
            return Ok();
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest resetRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetRequest.Email);
            if (user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                await _emailSender.SendPasswordResetCodeAsync(user, resetRequest.Email,
                    HtmlEncoder.Default.Encode(code));
            }

            return Ok();
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetRequest.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return ValidationProblem("Invalid token.");
            }

            IdentityResult result;
            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetRequest.ResetCode));
                result = await _userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }

            if (!result.Succeeded)
            {
                return ValidationProblem(result.Errors.First().Description);
            }

            return Ok();
        }

        [HttpPost("manage/2fa")]
        public async Task<IActionResult> ManageTwoFactor([FromBody] TwoFactorRequest tfaRequest)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (tfaRequest.Enable == true)
            {
                if (tfaRequest.ResetSharedKey)
                {
                    return ValidationProblem("CannotResetSharedKeyAndEnable",
                        "Resetting the 2fa shared key must disable 2fa until a 2fa token based on the new shared key is validated.");
                }
                else if (string.IsNullOrEmpty(tfaRequest.TwoFactorCode))
                {
                    return ValidationProblem("RequiresTwoFactor",
                        "No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa.");
                }
                else if (!await _userManager.VerifyTwoFactorTokenAsync(user,
                             _userManager.Options.Tokens.AuthenticatorTokenProvider, tfaRequest.TwoFactorCode))
                {
                    return ValidationProblem("InvalidTwoFactorCode",
                        "The 2fa token provided by the request was invalid. A valid 2fa token is required to enable 2fa.");
                }

                await _userManager.SetTwoFactorEnabledAsync(user, true);
            }
            else if (tfaRequest.Enable == false || tfaRequest.ResetSharedKey)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
            }

            if (tfaRequest.ResetSharedKey)
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
            }

            string[]? recoveryCodes = null;
            if (tfaRequest.ResetRecoveryCodes ||
                (tfaRequest.Enable == true && await _userManager.CountRecoveryCodesAsync(user) == 0))
            {
                var recoveryCodesEnumerable = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                recoveryCodes = recoveryCodesEnumerable?.ToArray();
            }

            if (tfaRequest.ForgetMachine)
            {
                await _signInManager.ForgetTwoFactorClientAsync();
            }

            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);

                if (string.IsNullOrEmpty(key))
                {
                    throw new NotSupportedException("The user manager must produce an authenticator key after reset.");
                }
            }

            return Ok(new TwoFactorResponse
            {
                SharedKey = key,
                RecoveryCodes = recoveryCodes,
                RecoveryCodesLeft = recoveryCodes?.Length ?? await _userManager.CountRecoveryCodesAsync(user),
                IsTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
            });
        }

        [HttpGet("manage/info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(await CreateInfoResponseAsync(user));
        }

        [HttpPost("manage/info")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] InfoRequest infoRequest)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(infoRequest.NewEmail) && !_emailAddressAttribute.IsValid(infoRequest.NewEmail))
            {
                return ValidationProblem("Invalid email.");
            }

            if (!string.IsNullOrEmpty(infoRequest.NewPassword))
            {
                if (string.IsNullOrEmpty(infoRequest.OldPassword))
                {
                    return ValidationProblem("OldPasswordRequired",
                        "The old password is required to set a new password. If the old password is forgotten, use /resetPassword.");
                }

                var changePasswordResult =
                    await _userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    return ValidationProblem(changePasswordResult.Errors.First().Description);
                }
            }

            if (!string.IsNullOrEmpty(infoRequest.NewEmail))
            {
                var email = await _userManager.GetEmailAsync(user);

                if (email != infoRequest.NewEmail)
                {
                    await SendConfirmationEmailAsync(user, infoRequest.NewEmail, isChange: true);
                }
            }

            return Ok(await CreateInfoResponseAsync(user));
        }

        [HttpPost("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin([FromBody] JwtHandler.ExternalAuthDto externalAuth)
        {
            var payload = await _jwtHandler.VerifyGoogleToken(externalAuth);
            if (payload == null)
                return BadRequest("Invalid External Authentication.");
            var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new User
                    {
                        Email = payload.Email,
                        UserName = payload.Email,
                        EmailConfirmed = true,
                        firstName = payload.GivenName
                    };
                    await _userManager.CreateAsync(user);
                }

                await _userManager.AddLoginAsync(user, info);
            }

            if (user == null)
                return BadRequest("Invalid External Authentication.");
            var result =
                await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
            if (!result.Succeeded)
                return BadRequest("Invalid External Authentication.");
            return Ok("Login successful.");
        }

        [HttpPost("logout"), Authorize]
        public async Task<IActionResult> Logout()
        {
            var principal = HttpContext.User;
            var principals = new ClaimsPrincipal(principal);
            var user = await _userManager.GetUserAsync(principals);
            Console.WriteLine(user);
            if (user != null)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }

            await _signInManager.SignOutAsync();
            return Ok("Logged out");
        }


        private async Task SendConfirmationEmailAsync(User user, string email, bool isChange = false)
        {
            var code = isChange
                ? await _userManager.GenerateChangeEmailTokenAsync(user, email)
                : await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var userId = await _userManager.GetUserIdAsync(user);
            var confirmEmailUrl = Url.Action("ConfirmEmail", "Identity",
                new { userId, code, changedEmail = isChange ? email : null }, Request.Scheme);
            await _emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));
        }

        private async Task<InfoResponse> CreateInfoResponseAsync(User user)
        {
            return new InfoResponse
            {
                Email = await _userManager.GetEmailAsync(user) ??
                        throw new NotSupportedException("Users must have an email."),
                IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user),
            };
        }
    }

    public class ExternalAuthDto
    {
        public string? Provider { get; set; }
        public string? IdToken { get; set; }
    }
}