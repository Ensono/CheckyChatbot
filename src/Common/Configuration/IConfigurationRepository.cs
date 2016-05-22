namespace Configuration {
    public interface IConfigurationRepository {
        string GetAppSetting(string key);
        string GetConnectionString(string key);
    }
}