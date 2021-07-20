using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Promitor.Parsers.Prometheus.Core;
using Promitor.Parsers.Prometheus.Core.Models.Interfaces;

namespace Microsoft.AspNetCore.Http
{
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Reads & parses an HTTP response as Prometheus metrics
        /// </summary>
        /// <param name="httpResponse">Http response message to parse</param>
        /// <returns>List of Prometheus metrics</returns>
        public static async Task<List<IMetric>> ReadAsPrometheusMetricsAsync(this HttpResponse httpResponse)
        {
            if(httpResponse == null || httpResponse.Body == null)
            {
                return Enumerable.Empty<IMetric>().ToList();
            }

            var metrics = await PrometheusMetricsParser.ParseAsync(httpResponse.Body);
            return metrics;
        }
    }
}
