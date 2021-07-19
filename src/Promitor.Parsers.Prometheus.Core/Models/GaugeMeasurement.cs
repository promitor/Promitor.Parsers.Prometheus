using System;
using System.Collections.Generic;

namespace Promitor.Parsers.Prometheus.Core.Models
{
    public class GaugeMeasurement
    {
        public Dictionary<string, string> Labels { get; set; } = new Dictionary<string, string>();
        public double Value { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
