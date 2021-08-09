using System;
using Promitor.Parsers.Prometheus.Core;
using Promitor.Parsers.Prometheus.Core.Models;
using Promitor.Parsers.Prometheus.Core.Models.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Promitor.Parsers.Prometheus.Tests
{
    [Category("Unit")]
    public class PrometheusMetricsParserTests
    {
        [Theory]
        [InlineData(52977208)]
        [InlineData(153.1351)]
        [InlineData(-52977208)]
        [InlineData(-153.1351)]
        [InlineData(-1)]
        [InlineData(double.NaN)]
        public async Task Parse_RawMetricWithTimestampAndLabels_ReturnCorrectInfo(double metricValue)
        {
            // Arrange
            var metricName = "azure_container_registry_total_pull_count_discovered";
            var metricDescription = "Amount of images that were pulled from the container registry";
            var resourceGroupName = "promitor";
            var subscriptionId = "0f9d7fea-99e8-4768-8672-06a28514f77e";
            var resourceUri = "subscriptions/0f9d7fea-99e8-4768-8672-06a28514f77e/resourceGroups/promitor/providers/Microsoft.ContainerRegistry/registries/promitor";
            var instanceName = "promitor";
            var timestamp = DateTimeOffset.UtcNow;
            var rawMetric = $@"# HELP {metricName} {metricDescription}
# TYPE {metricName} gauge
{metricName}{{resource_group=""{resourceGroupName}"",subscription_id=""{subscriptionId}"",resource_uri=""{resourceUri}"",instance_name=""{instanceName}""}} {metricValue} {timestamp.ToUnixTimeMilliseconds()}";
            var rawMetricsStream = GenerateStream(rawMetric);

            // Act
            var metrics = await PrometheusMetricsParser.ParseAsync(rawMetricsStream);

            // Assert
            Assert.True(rawMetricsStream.CanRead);
            Assert.True(rawMetricsStream.CanSeek);
            Assert.NotNull(metrics);
            Assert.Single(metrics);
            var testMetric = metrics.FirstOrDefault();
            Assert.NotNull(testMetric);
            Assert.Equal(metricName, testMetric.Name);
            Assert.Equal(metricDescription, testMetric.Description);
            Assert.Equal(MetricTypes.Gauge, testMetric.Type);
            var testGauge = testMetric as Gauge;
            Assert.Single(testGauge.Measurements);
            var testMeasurement = testGauge.Measurements.First();
            Assert.Equal(metricValue, testMeasurement.Value);
            Assert.Equal(timestamp.ToString("yyyy-MM-ddTHH:mm:ss.zzz"), testMeasurement.Timestamp?.ToString("yyyy-MM-ddTHH:mm:ss.zzz"));
            Assert.NotNull(testMeasurement.Labels);
            Assert.Equal(4, testMeasurement.Labels.Count);
            Assert.True(testMeasurement.Labels.ContainsKey("resource_group"));
            Assert.True(testMeasurement.Labels.ContainsKey("subscription_id"));
            Assert.True(testMeasurement.Labels.ContainsKey("resource_uri"));
            Assert.True(testMeasurement.Labels.ContainsKey("instance_name"));
            Assert.Equal(resourceGroupName, testMeasurement.Labels["resource_group"]);
            Assert.Equal(subscriptionId, testMeasurement.Labels["subscription_id"]);
            Assert.Equal(resourceUri, testMeasurement.Labels["resource_uri"]);
            Assert.Equal(instanceName, testMeasurement.Labels["instance_name"]);
        }

        [Fact]
        public async Task Parse_RawMetricWithTimestampButWithoutLabels_ReturnCorrectInfo()
        {
            // Arrange
            var metricName = "promitor_runtime_dotnet_totalmemory";
            var metricDescription = "Total known allocated memory";
            double metricValue = 52977208;
            var timestamp = DateTimeOffset.UtcNow;
            var rawMetric = $@"# HELP {metricName} {metricDescription}
# TYPE {metricName} gauge
{metricName} {metricValue} {timestamp.ToUnixTimeMilliseconds()}";
            var rawMetricsStream = GenerateStream(rawMetric);

            // Act
            var metrics = await PrometheusMetricsParser.ParseAsync(rawMetricsStream);

            // Assert
            Assert.True(rawMetricsStream.CanRead);
            Assert.True(rawMetricsStream.CanSeek);
            Assert.NotNull(metrics);
            Assert.Single(metrics);
            var testMetric = metrics.FirstOrDefault();
            Assert.NotNull(testMetric);
            Assert.Equal(metricName, testMetric.Name);
            Assert.Equal(metricDescription, testMetric.Description);
            Assert.Equal(MetricTypes.Gauge, testMetric.Type);
            var testGauge = testMetric as Gauge;
            Assert.Single(testGauge.Measurements);
            var testMeasurement = testGauge.Measurements.First();
            Assert.Equal(metricValue, testMeasurement.Value);
            Assert.Equal(timestamp.ToString("yyyy-MM-ddTHH:mm:ss.zzz"), testMeasurement.Timestamp?.ToString("yyyy-MM-ddTHH:mm:ss.zzz"));
            Assert.NotNull(testMeasurement.Labels);
            Assert.Empty(testMeasurement.Labels);
        }

        [Fact]
        public async Task Parse_RawMetricWithoutLabelsAndTimestamp_ReturnCorrectInfo()
        {
            // Arrange
            var metricName = "promitor_runtime_dotnet_totalmemory";
            var metricDescription = "Total known allocated memory";
            double metricValue = 52977208;
            var rawMetric = $@"# HELP {metricName} {metricDescription}
# TYPE {metricName} gauge
{metricName} {metricValue}";
            var rawMetricsStream = GenerateStream(rawMetric);

            // Act
            var metrics = await PrometheusMetricsParser.ParseAsync(rawMetricsStream);

            // Assert
            Assert.True(rawMetricsStream.CanRead);
            Assert.True(rawMetricsStream.CanSeek);
            Assert.NotNull(metrics);
            Assert.Single(metrics);
            var testMetric = metrics.FirstOrDefault();
            Assert.NotNull(testMetric);
            Assert.Equal(metricName, testMetric.Name);
            Assert.Equal(metricDescription, testMetric.Description);
            Assert.Equal(MetricTypes.Gauge, testMetric.Type);
            var testGauge = testMetric as Gauge;
            Assert.Single(testGauge.Measurements);
            var testMeasurement = testGauge.Measurements.First();
            Assert.Null(testMeasurement.Timestamp);
            Assert.Equal(metricValue, testMeasurement.Value);
            Assert.NotNull(testMeasurement.Labels);
            Assert.Empty(testMeasurement.Labels);
        }

        [Fact]
        public async Task Parse_ValidInputWithLabels_ReturnsMetrics()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Samples", "raw-metrics-with-labels.txt");
            var rawMetricsStream = File.OpenRead(filePath);

            // Act
            var metrics = await PrometheusMetricsParser.ParseAsync(rawMetricsStream);

            // Assert
            Assert.True(rawMetricsStream.CanRead);
            Assert.True(rawMetricsStream.CanSeek);
            Assert.NotNull(metrics);
            AssertAzureContainerRegistryPullCount(metrics);
        }

        [Fact]
        public async Task Parse_ValidInputWithoutLabels_ReturnsMetrics()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Samples", "raw-metrics-without-labels.txt");
            var rawMetricsStream = File.OpenRead(filePath);

            // Act
            var metrics = await PrometheusMetricsParser.ParseAsync(rawMetricsStream);

            // Assert
            Assert.True(rawMetricsStream.CanRead);
            Assert.True(rawMetricsStream.CanSeek);
            Assert.NotNull(metrics);
        }

        private void AssertAzureContainerRegistryPullCount(List<IMetric> metrics)
        {
            var acrMetric = metrics.Find(f => f.Name == "azure_container_registry_total_pull_count_discovered");
            Assert.NotNull(acrMetric);
            Assert.Equal(MetricTypes.Gauge, acrMetric.Type);
            Assert.Equal("Amount of images that were pulled from the container registry", acrMetric.Description);
            var acrGauge = acrMetric as Gauge;
            Assert.Equal(2, acrGauge.Measurements.Count);
            var firstMeasurement = acrGauge.Measurements.First();
            Assert.NotNull(firstMeasurement.Labels);
            Assert.Equal(4, firstMeasurement.Labels.Count);
            Assert.True(firstMeasurement.Labels.ContainsKey("resource_group"));
            Assert.True(firstMeasurement.Labels.ContainsKey("subscription_id"));
            Assert.True(firstMeasurement.Labels.ContainsKey("resource_uri"));
            Assert.True(firstMeasurement.Labels.ContainsKey("instance_name"));
        }
        public static Stream GenerateStream(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
