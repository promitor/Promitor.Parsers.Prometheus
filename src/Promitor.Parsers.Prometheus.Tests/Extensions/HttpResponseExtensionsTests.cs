using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Promitor.Parsers.Prometheus.Tests.Extensions
{
    [Category("Unit")]
    public class HttpResponseExtensionsTests
    {
        [Fact]
        public async Task ReadAsPrometheusMetricsAsync_ValidInput_ReturnsMetric()
        {
            // Arrange
            var rawMetric = @"# HELP azure_container_registry_total_pull_count_discovered Amount of images that were pulled from the container registry
# TYPE azure_container_registry_total_pull_count_discovered gauge
azure_container_registry_total_pull_count_discovered{resource_group = ""promitor"",subscription_id = ""0f9d7fea-99e8-4768-8672-06a28514f77e"",resource_uri = ""subscriptions/0f9d7fea-99e8-4768-8672-06a28514f77e/resourceGroups/promitor/providers/Microsoft.ContainerRegistry/registries/promitor"",instance_name = ""promitor""} -1 1605802323456
azure_container_registry_total_pull_count_discovered{resource_group = ""open-source-projects"",subscription_id = ""0f9d7fea-99e8-4768-8672-06a28514f77e"",resource_uri = ""subscriptions/0f9d7fea-99e8-4768-8672-06a28514f77e/resourceGroups/open-source-projects/providers/Microsoft.ContainerRegistry/registries/tomkerkhove"",instance_name = ""tomkerkhove""} -1 1605802326606";
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            await httpContext.Response.WriteAsync(rawMetric);
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            // Act
            var metrics = await httpContext.Response.ReadAsPrometheusMetricsAsync();

            // Assert
            Assert.NotNull(metrics);
            Assert.Single(metrics);
        }

        [Fact]
        public async Task ReadAsPrometheusMetricsAsync_ResponseIsNull_ReturnEmptyList()
        {
            // Act
            var metrics = await ((HttpResponse) null).ReadAsPrometheusMetricsAsync();

            // Assert
            Assert.NotNull(metrics);
            Assert.Empty(metrics);
        }
    }
}
