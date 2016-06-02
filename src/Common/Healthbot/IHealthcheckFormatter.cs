using System.Collections.Generic;

namespace Checky.Common.Healthbot {
    public interface IHealthcheckFormatter {
        Status GetOverallState(IEnumerable<Check> checks);
        string Render(string environment, string service, Healthcheck healthcheck);
    }
}