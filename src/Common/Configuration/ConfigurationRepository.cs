using System;
using System.Configuration;

namespace Configuration {
    public class ConfigurationRepository : IConfigurationRepository {
        public string GetAppSetting(string key) {
            var configuration = ConfigurationManager.AppSettings;
            var authToken = configuration[key];

            if (string.IsNullOrWhiteSpace(authToken)) {
                authToken = Environment.GetEnvironmentVariable($"APPSETTING_{key}");
            }
            return authToken;
        }

        public string GetConnectionString(string key) {
            var configuration = ConfigurationManager.ConnectionStrings;
            var datasource = configuration[key];

            var connectionString = datasource == null
                ? Environment.GetEnvironmentVariable($"CONNECTIONSTRING_{key}")
                : datasource.ConnectionString;

            if (connectionString == null) {
                throw new ConfigurationErrorsException("Unable to get connection string for DocumentDB.");
            }
            return connectionString;
        }
    }
}