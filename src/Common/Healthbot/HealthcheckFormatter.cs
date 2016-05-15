using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthbot {
    public class HealthcheckFormatter {
        private Dictionary<Status, string> emojiLookup = new Dictionary<Status, string> {
            {Status.Up, ":green_heart: Up"},
            {Status.Degraded, ":yellow_heart: Degraded"},
            {Status.Down, ":red_heart: Down"}
        };

        public string Render(string environment, string service, Healthcheck healthcheck) {
            var builder = new StringBuilder();

            var aggregateState = GetOverallState(healthcheck.Checks);
            var aggregateStateText = emojiLookup[aggregateState];
            builder.AppendLine(
                $"{service} on {environment} is running version {healthcheck.Version} and has a status of {aggregateStateText}.");
            if (aggregateState == Status.Degraded || aggregateState == Status.Down) {
                foreach (var check in healthcheck.Checks) {
                    var checkState = emojiLookup[check.Status];
                    if (String.IsNullOrWhiteSpace(check.ExtraInformation)) {
                        builder.AppendLine($"  {check.Description} {checkState}");
                    } else {
                        builder.AppendLine($"  {check.Description} {checkState} ({check.ExtraInformation})");
                    }
                }
            }

            return builder.ToString();
        }

        public Status GetOverallState(IEnumerable<Check> checks) {
            if (checks.Any(x => x.Status == Status.Down)) {
                return Status.Down;
            }

            if (checks.Any(x => x.Status == Status.Degraded)) {
                return Status.Degraded;
            }

            return Status.Up;
        }
    }
}