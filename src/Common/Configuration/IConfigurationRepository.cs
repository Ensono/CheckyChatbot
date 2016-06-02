namespace Checky.Common.Configuration {
    public interface IConfigurationRepository {
        string GetAppSetting(string key);
        string GetConnectionString(string key, ConnectionStringType type = ConnectionStringType.Custom);
    }
}