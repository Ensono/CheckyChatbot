using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace Checky.Common.Healthbot {
    public class HealthcheckClient : IHealthcheckClient {
        private readonly HttpClient _client = new HttpClient();

        public Healthcheck GetHealth(Uri baseUri) {
            var version = _client.GetStringAsync($"{baseUri}/version");
            var healthcheck = _client.GetStringAsync($"{baseUri}/healthcheck");

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