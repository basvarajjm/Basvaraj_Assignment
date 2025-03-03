using WebApp.Models;

namespace WebApp.Helpers
{
    public static class ConfigurationHelper
    {
        private const string _authConfig = "AuthConfig";
        private const string _authKey = "Key";
        private const string _authIssuer = "Issuer";

        private const string _dkBankConfig = "DKBank";
        private const string _dkBaseUrl = "BaseUrl";

        internal static bool TryToGetAuthConfiguration(IConfiguration configuration, out AuthConfig? authConfig)
        {
            authConfig = null;
            if (!IsAuthConfigurationSet(configuration))
            {
                return false;
            }
            authConfig = new AuthConfig();
            configuration.GetSection(_authConfig).Bind(authConfig);
            return true;
        }

        internal static bool IsAllRequiredConfigurationSet(IConfiguration configuration)
            => IsDKBankConfigurationSet(configuration) && IsAuthConfigurationSet(configuration);

        internal static bool IsAuthConfigurationSet(IConfiguration configuration)
            => !string.IsNullOrEmpty(configuration[$"{_authConfig}:{_authKey}"]) && !string.IsNullOrEmpty(configuration[$"{_authConfig}:{_authIssuer}"]);

        internal static bool IsDKBankConfigurationSet(IConfiguration configuration)
            => !string.IsNullOrEmpty(configuration[$"{_dkBankConfig}:{_dkBaseUrl}"]);
    }
}
