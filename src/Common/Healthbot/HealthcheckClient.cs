using System;
using Checky.Common.Network;
using Newtonsoft.Json;

namespace Checky.Common.Healthbot {
    public class HealthcheckClient : IHealthcheckClient {
        private readonly IHttpClientFactory _httpClientFactory;

        public HealthcheckClient(IHttpClientFactory httpClientFactory) {
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
            _httpClientFactory = httpClientFactory;
        }

        public Healthcheck GetHealth(Uri baseUri, string expectedServerCertificateSubject = null) {
            var client = _httpClientFactory.GetClient(expectedServerCertificateSubject);
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