using Promitor.Parsers.Prometheus.Core.Models.Interfaces;
using System.Collections.Generic;

namespace Promitor.Parsers.Prometheus.Core.Models
{
    public class Gauge : IMetric
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public MetricTypes Type => MetricTypes.Gauge;
        public List<GaugeMeasurement> Measurements { get; set; } = new List<GaugeMeasurement>();

        public override string ToString()
        {
            return $"{Name} - {Description}";
        }
    }
}
