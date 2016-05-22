using System.Collections.Generic;

namespace Healthbot {
    public interface IHealthcheckFormatter {
        Status GetOverallState(IEnumerable<Check> checks);
        string Render(string environment, string service, Healthcheck healthcheck);
    }
}