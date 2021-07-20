using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Promitor.Parsers.Prometheus.Tests.Extensions
{
    [Category("Unit")]
    public class HttpResponseMessageExtensionsTests
    {
        [Fact]
        public async Task ReadAsPrometheusMetricsAsync_ValidInput_ReturnsMetric()
        {
            // Arrange
            var rawMetric = @"# HELP azure_container_registry_total_pull_count_discovered Amount of images that were pulled from the container registry
# TYPE azure_container_registry_total_pull_count_discovered gauge
azure_container_registry_total_pull_count_discovered{resource_group = ""promitor"",subscription_id = ""0f9d7fea-99e8-4768-8672-06a28514f77e"",resource_uri = ""subscriptions/0f9d7fea-99e8-4768-8672-06a28514f77e/resourceGroups/promitor/providers/Microsoft.ContainerRegistry/registries/promitor"",instance_name = ""promitor""} -1 1605802323456
azure_container_registry_total_pull_count_discovered{resource_group = ""open-source-projects"",subscription_id = ""0f9d7fea-99e8-4768-8672-06a28514f77e"",resource_uri = ""subscriptions/0f9d7fea-99e8-4768-8672-06a28514f77e/resourceGroups/open-source-projects/providers/Microsoft.ContainerRegistry/registries/tomkerkhove"",instance_name = ""tomkerkhove""} -1 1605802326606";
            var responseStream = GenerateStreamFromString(rawMetric);
            var httpResponseMessage = new HttpResponseMessage
            {
                Content = new StreamContent(responseStream)
            };

            // Act
            var metrics = await httpResponseMessage.ReadAsPrometheusMetricsAsync();

            // Assert
            Assert.NotNull(metrics);
            Assert.Single(metrics);
        }

        [Fact]
        public async Task ReadAsPrometheusMetricsAsync_NoContent_ReturnEmptyList()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                Content = null
            };

            // Act
            var metrics = await httpResponseMessage.ReadAsPrometheusMetricsAsync();

            // Assert
            Assert.NotNull(metrics);
            Assert.Empty(metrics);
        }

        [Fact]
        public async Task ReadAsPrometheusMetricsAsync_ResponseIsNull_ReturnEmptyList()
        {
            // Act
            var metrics = await ((HttpResponseMessage) null).ReadAsPrometheusMetricsAsync();

            // Assert
            Assert.NotNull(metrics);
            Assert.Empty(metrics);
        }

        private Stream GenerateStreamFromString(string rawInput)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(rawInput);
            streamWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
