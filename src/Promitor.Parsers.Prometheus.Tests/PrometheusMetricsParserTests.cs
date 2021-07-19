using Promitor.Parsers.Prometheus.Core;
using Promitor.Parsers.Prometheus.Core.Models;
using Promitor.Parsers.Prometheus.Core.Models.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Promitor.Parsers.Prometheus.Tests
{
    [Category("Unit")]
    public class PrometheusMetricsParserTests
    {
        [Fact]
        public async Task Parse_ValidInput_ReturnsMetrics()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Samples", "raw-metrics.txt");
            var rawMetricsStream = File.OpenRead(filePath);

            // Act
            var metrics = await PrometheusMetricsParser.ParseAsync(rawMetricsStream);

            // Assert
            Assert.True(rawMetricsStream.CanRead);
            Assert.True(rawMetricsStream.CanSeek);
            Assert.NotNull(metrics);
            AssertAzureContainerRegistryPullCount(metrics);
        }

        private void AssertAzureContainerRegistryPullCount(List<IMetric> metrics)
        {
            var acrMetric = metrics.Find(f => f.Name == "azure_container_registry_total_pull_count_discovered");
            Assert.NotNull(acrMetric);
            Assert.Equal(MetricTypes.Gauge, acrMetric.Type);
            Assert.Equal("Amount of images that were pulled from the container registry", acrMetric.Description);
            var acrGauge = acrMetric as Gauge;
            Assert.Equal(2, acrGauge.Measurements.Count);
        }
    }
}
