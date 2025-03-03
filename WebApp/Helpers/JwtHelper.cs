using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApp.Models;

namespace WebApp.Helpers
{
    public static class JwtHelper
    {
        public static Action<JwtBearerOptions> GetJwtOptionAction(AuthConfig authConfig)
        {
            return options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience =  false,
                    ValidIssuer = authConfig.Issuer,
                    // We shouldn't store the KEY in some appsettings files as it contains sensitive data.
                    // Maybe we can use something like Environment variables or Vault in this case.
                    // For this example I have used appsettings file for storing Key (We can assume that this value is being taken from secure storage like Vault).
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(authConfig.Key))
                };
            };
        }
    }
}
