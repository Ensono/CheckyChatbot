using System;

namespace Healthbot {
    public interface IHealthcheckClient {
        Healthcheck GetHealth(Uri baseUri);
    }
}