/*using System.Security.Claims;
using AppGateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Entities;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _context;
        private readonly JwtHandler _jwtHandler;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            AppDbContext context, JwtHandler jwtHandler, ILogger<AccountController> logger)
        {
            _jwtHandler = jwtHandler;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [HttpPost("test")]
        public async Task<List<User>> Test()
        {
            var res = await _context.Users.ToListAsync();
            return res;
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

        /*
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                firstName = model.FirstName,
                lastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Send email verification
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                // Send the confirmation link via email (implementation not shown)
                await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your account by clicking this link: token : {token} and email : {user.Id}");

                return Ok("Registration successful. Please check your email to confirm your account.");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }#1#
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

        /*
        [HttpPost("enable-mfa"), Authorize]
        public async Task<IActionResult> EnableMfa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            await _emailSender.SendEmailAsync(user.Email, "MFA Code", $"Your MFA code is: {token}");

            return Ok("MFA enabled. Check your email for the verification code.");
        }

        [HttpPost("verify-mfa"), Authorize]
        public async Task<IActionResult> VerifyMfa([FromBody] string token)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", token);
            if (result)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                return Ok("MFA verified successfully.");
            }

            return BadRequest("Invalid MFA code.");
        }
        #1#

        [HttpPost("login-mfa")]
        public async Task<IActionResult> LoginMfa([FromBody] VerifyMfaModel model)
        {
            _logger.LogInformation("Starting MFA login process for email: {Email}", model.Email);

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", model.Email);
                return Unauthorized("Invalid MFA attempt.");
            }

            // Perform the MFA sign-in
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(model.Token, model.RememberMe, false);
            if (result.Succeeded)
            {
                _logger.LogInformation("MFA verification and login successful for email: {Email}", model.Email);
                return Ok("MFA verification and login successful.");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out for email: {Email}", model.Email);
                return Unauthorized("User account locked out.");
            }

            if (result.IsNotAllowed)
            {
                _logger.LogWarning("User account not allowed for email: {Email}", model.Email);
                return Unauthorized("User account not allowed.");
            }

            _logger.LogWarning("Invalid MFA attempt for email: {Email}", model.Email);
            return Unauthorized("Invalid MFA attempt.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid login attempt.");
            if (!user.EmailConfirmed)
            {
                return Unauthorized("Please confirm your email.");
            }

            var result =
                await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe,
                    lockoutOnFailure: true);
            if (result.Succeeded)
            {
                var isMfaEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
                if (isMfaEnabled)
                {
                    return Ok(new LoginResponse()
                    {
                        RequiresMfa = true
                    });
                }

                return Ok("Login successful.");
            }

            if (result.RequiresTwoFactor)
            {
                return Ok(new LoginResponse()
                {
                    RequiresMfa = true
                });
            }

            if (result.IsLockedOut)
            {
                return Unauthorized("User account locked out.");
            }

            return Unauthorized("Invalid login attempt.");
        }

        /*
        [HttpPost("verify-mfa-auth")]
        public async Task<IActionResult> VerifyMfa([FromBody] VerifyMfaModel model)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid MFA attempt.");
            }

            // Verify the MFA token
            var isTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Authenticator", model.Token);
            if (isTokenValid)
            {
                return Ok("MFA verification successful.");
            }

            return Unauthorized("Invalid MFA token.");
        }
        #1#
    }

    public class BaseAuth
    {
        public string Email { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
    }

    public class LoginModel : BaseAuth
    {
        public string Password { get; set; } = string.Empty;
    }

    /*public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }#1#
    public class LoginResponse
    {
        public bool RequiresMfa { get; set; } = false;
    }

    public class VerifyMfaModel : BaseAuth
    {
        public string Token { get; set; } = string.Empty;
    }
}*/