using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace Healthbot {
    public class HealthcheckClient {
        private readonly HttpClient client = new HttpClient();

        public Healthcheck GetHealth(Uri baseUri) {
            var version = client.GetStringAsync($"{baseUri}/version");
            var healthcheck = client.GetStringAsync($"{baseUri}/healthcheck");

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new TimeSpanConverter());

            var versionResult = JsonConvert.DeserializeObject<DeployedVersion>(version.Result);
            var healthcheckResult = JsonConvert.DeserializeObject<DeployedHealthcheck>(healthcheck.Result, settings);

            return new Healthcheck {
                Version = versionResult.Version,
                ResponseTime = healthcheckResult.TotalResponseTime,
                Checks = healthcheckResult.Checks
            };
        }
    }
}