using System;
using System.Collections.Generic;

namespace Promitor.Parsers.Prometheus.Core.Models
{
    public class Gauge
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<GaugeMeasurement> Measurements { get; set; } = new List<GaugeMeasurement>();

        public override string ToString()
        {
            return $"{Name} - {Description}";
        }
    }
}
