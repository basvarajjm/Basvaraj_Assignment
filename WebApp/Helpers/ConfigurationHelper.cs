using WebApp.Models;

namespace WebApp.Helpers
{
    public static class ConfigurationHelper
    {
        private const string _authConfigKey = "AuthConfig";
        private const string _authKey = "Key";
        private const string _authIssuer = "Issuer";

        internal static bool TryToGetAuthConfiguration(IConfiguration configuration, out AuthConfig authConfig)
        {
            authConfig = null;
            if (!IsAllRequiredConfigurationSet(configuration))
            {
                return false;
            }
            authConfig = new AuthConfig();
            configuration.GetSection(_authConfigKey).Bind(authConfig);
            return true;
        }

        internal static bool IsAllRequiredConfigurationSet(IConfiguration configuration)
            => !string.IsNullOrEmpty(configuration[$"{_authConfigKey}:{_authKey}"]) && !string.IsNullOrEmpty(configuration[$"{_authConfigKey}:{_authIssuer}"]);
    }
}
