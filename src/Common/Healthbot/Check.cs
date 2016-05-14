using System;

namespace Healthbot {
    public class Check {
        public string Description { get; set; }
        public string ExtraInformation { get; set; }
        public Status Status { get; set; }
        public TimeSpan responseTime { get; set; }
    }
}