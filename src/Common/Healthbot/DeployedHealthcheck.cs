using System;
using System.Collections.Generic;

namespace Healthbot {
    public class DeployedHealthcheck {
        public TimeSpan TotalResponseTime { get; set; }
        public IEnumerable<Check> Checks { get; set; }
    }
}