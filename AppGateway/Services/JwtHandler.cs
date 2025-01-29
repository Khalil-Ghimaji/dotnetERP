using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Persistence.Entities;

namespace AppGateway.Services;

public class JwtHandler
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    public JwtHandler(IConfiguration configuration, UserManager<User> userManager)
    {
        _userManager = userManager;
        _configuration = configuration;
    }
    public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _configuration["Authentication:Google:ClientId"] }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
            return payload;
        }
        catch (Exception ex)
        {
            //log an exception
            return null;
        }
    }
    public class ExternalAuthDto
    {
        public string? Provider { get; set; }
        public string? IdToken { get; set; }
    }
}
