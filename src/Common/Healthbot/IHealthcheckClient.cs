using System;

namespace Checky.Common.Healthbot {
    public interface IHealthcheckClient {
        Healthcheck GetHealth(Uri baseUri);
    }
}