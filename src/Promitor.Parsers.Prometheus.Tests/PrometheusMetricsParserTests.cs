using Promitor.Parsers.Prometheus.Core;
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

        private void AssertAzureContainerRegistryPullCount(List<Core.Models.Gauge> metrics)
        {
            var acrMetric = metrics.Find(f => f.Name == "azure_container_registry_total_pull_count_discovered");
            Assert.NotNull(acrMetric);
            Assert.Equal("Amount of images that were pulled from the container registry", acrMetric.Description);
            Assert.Equal(2, acrMetric.Measurements.Count);
        }
    }
}
