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

        public string GetConnectionString(string key, ConnectionStringType type = ConnectionStringType.Custom) {
            var configuration = ConfigurationManager.ConnectionStrings;
            var datasource = configuration[key];

            var prefix = GetConnectionStringPrefix(type);
            var connectionString = datasource == null
                ? Environment.GetEnvironmentVariable($"{prefix}{key}")
                : datasource.ConnectionString;

            if (connectionString == null) {
                throw new ConfigurationErrorsException("Unable to get connection string for DocumentDB.");
            }
            return connectionString;
        }

        private string GetConnectionStringPrefix(ConnectionStringType connStrType) {
            switch (connStrType) {
                case ConnectionStringType.SqlAzure:
                    return "SQLAZURECONNSTR_";
                case ConnectionStringType.SqlServer:
                    return "SQLCONNSTR_";
                case ConnectionStringType.MySql:
                    return "MYSQLCONNSTR_";
                case ConnectionStringType.Custom:
                    return "CUSTOMCONNSTR_";
                default:
                    throw new ArgumentOutOfRangeException(nameof(connStrType), connStrType, null);
            }
        }
    }
}