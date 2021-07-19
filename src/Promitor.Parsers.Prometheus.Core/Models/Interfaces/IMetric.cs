namespace Promitor.Parsers.Prometheus.Core.Models.Interfaces
{
    public interface IMetric
    {
        public string Name { get; }
        public string Description { get; }
        public MetricTypes Type { get; }
    }
}
