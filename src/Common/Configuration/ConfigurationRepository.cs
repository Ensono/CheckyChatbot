using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ComponentModel;

namespace Configuration {
    public class ConfigurationRepository : IConfigurationRepository {
        private readonly IHelpers _helpers;

        public ConfigurationRepository(IHelpers helpers) {
            _helpers = helpers;
        }

        public string GetAppSetting(string key) {
            var configuration = ConfigurationManager.AppSettings;
            var configContainsKey = configuration.AllKeys.Contains(key);
            var valueFromConfig = configuration[key];
            var valueFromConfigIsValid = !string.IsNullOrWhiteSpace(valueFromConfig);
            var environmentVariableKey = $"APPSETTING_{key}";
            var environmentVariables = Environment.GetEnvironmentVariables();
            var environmentVariableExists = environmentVariables.Contains(environmentVariableKey);
            var valueFromEnvironment = environmentVariables[environmentVariableKey] as string;
            var valueFromEnvironmentIsValid = !string.IsNullOrWhiteSpace(valueFromEnvironment);

            if (configContainsKey && valueFromConfigIsValid) {
                return valueFromConfig;
            }

            if (environmentVariableExists && valueFromEnvironmentIsValid) {
                return valueFromEnvironment;
            }

            var messages = new List<string>();
            if (!configContainsKey && !environmentVariableExists) {
                messages.Add(
                    $"the specified key does not exist in either configuraiton as '{key}' or environment variables as '{environmentVariableKey}'.");
            } else {
                if (configContainsKey) {
                    messages.Add(
                        $"the specified key exists in the configuration as '{key}' however the value was not valid, {_helpers.Mask(valueFromConfig)}");
                }

                if (environmentVariableExists) {
                    messages.Add(
                        $"the specified key exists in environment variables as '{environmentVariableKey}' however the value was not valid: {_helpers.Mask(valueFromEnvironment)}");
                }
            }

            var formattedMessages = messages
                .Select(x => $"{Environment.NewLine} - {x}")
                .Aggregate((current, next) => current + next);

            throw new InvalidOperationException(
                $"An attempt to read the '{key}' configuration value: {formattedMessages}");
        }

        public string GetConnectionString(string key, ConnectionStringType type = ConnectionStringType.Custom) {
            var configuration = ConfigurationManager.ConnectionStrings;
            var datasource = configuration[key];

            var prefix = GetConnectionStringPrefix(type);
            var connectionString = datasource == null
                ? Environment.GetEnvironmentVariable($"{prefix}{key}")
                : datasource.ConnectionString;

            if (connectionString == null) {
                throw new ConfigurationErrorsException($"Unable to get connection string identified by '{key}'.");
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