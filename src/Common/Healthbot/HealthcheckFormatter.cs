using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Healthbot {
    public class HealthcheckFormatter : IHealthcheckFormatter {
        private readonly Dictionary<Status, string> _emojiLookup = new Dictionary<Status, string> {
            {Status.Up, ":green_heart: Up"},
            {Status.Degraded, ":yellow_heart: Degraded"},
            {Status.Down, ":red_heart: Down"}
        };

        public string Render(string environment, string service, Healthcheck healthcheck) {
            var builder = new StringBuilder();

            var aggregateState = GetOverallState(healthcheck.Checks);
            var aggregateStateText = _emojiLookup[aggregateState];
            builder.AppendLine(
                $"{service} on {environment} is running version {healthcheck.Version} and has a status of {aggregateStateText}.");
            if (aggregateState == Status.Degraded || aggregateState == Status.Down) {
                foreach (var check in healthcheck.Checks) {
                    var checkState = _emojiLookup[check.Status];
                    if (string.IsNullOrWhiteSpace(check.ExtraInformation)) {
                        builder.AppendLine($"  {check.Description} {checkState}");
                    } else {
                        builder.AppendLine($"  {check.Description} {checkState} ({check.ExtraInformation})");
                    }
                }
            }

            return builder.ToString();
        }

        public Status GetOverallState(IEnumerable<Check> checks) {
            var enumerable = checks as Check[] ?? checks.ToArray();
            if (enumerable.Any(x => x.Status == Status.Down)) {
                return Status.Down;
            }

            return enumerable.Any(x => x.Status == Status.Degraded) ? Status.Degraded : Status.Up;
        }
    }
}