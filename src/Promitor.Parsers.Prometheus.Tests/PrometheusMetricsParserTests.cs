using Promitor.Parsers.Prometheus.Core;
using System;
using System.ComponentModel;
using System.IO;
using Xunit;

namespace Promitor.Parsers.Prometheus.Tests
{
    [Category("Unit")]
    public class PrometheusMetricsParserTests
    {
        [Fact]
        public void Parse_ValidInput_ReturnsMetrics()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Samples", "raw-metrics.txt");
            var rawMetrics = File.ReadAllText(filePath);

            // Act
            var metrics = PrometheusMetricsParser.Parse(rawMetrics);

            // Assert
            Assert.NotNull(metrics);
        }
    }
}
