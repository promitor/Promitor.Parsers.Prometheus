using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Promitor.Parsers.Prometheus.Core;
using Promitor.Parsers.Prometheus.Core.Models.Interfaces;

namespace System.Net.Http
{
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Reads & parses an HTTP response as Prometheus metrics
        /// </summary>
        /// <param name="httpResponseMessage">Http response message to parse</param>
        /// <returns>List of Prometheus metrics</returns>
        public static async Task<List<IMetric>> ReadAsPrometheusMetricsAsync(this HttpResponseMessage httpResponseMessage)
        {
            if(httpResponseMessage == null || httpResponseMessage.Content == null)
            {
                return Enumerable.Empty<IMetric>().ToList();
            }

            var responseStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var metrics = await PrometheusMetricsParser.ParseAsync(responseStream);
            return metrics;
        }
    }
}
