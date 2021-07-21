using Promitor.Parsers.Prometheus.Core.Models;
using Promitor.Parsers.Prometheus.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Promitor.Parsers.Prometheus.Core
{
    public class PrometheusMetricsParser
    {
        const string MetricInfoRegex = @"# (\w+) (\w*) (.*)";
        const string MeasurementRegex = @"(.+){(.*)} ((?:-?\d+(?:\.\d*)*)*(?:NaN)*) (\d*)";

        public static async Task<List<IMetric>> ParseAsync(Stream rawMetricsStream)
        {
            var originalPosition = rawMetricsStream.Position;

            // Go to the beginning of the stream
            if(rawMetricsStream.CanSeek)
            {
                rawMetricsStream.Seek(0, SeekOrigin.Begin);
            }

            // Interpret the metrics
            var metrics = await InterpretRawMetricsStreamAsync(rawMetricsStream);

            // Reset stream to original position
            if(rawMetricsStream.CanSeek)
            {
                rawMetricsStream.Seek(originalPosition, SeekOrigin.Begin);
            }

            return metrics;
        }

        private static async Task<List<IMetric>> InterpretRawMetricsStreamAsync(Stream rawMetricsStream)
        {
            var metrics = new List<IMetric>();
            var streamReader = new StreamReader(rawMetricsStream);
            
            Gauge unfinishedGauge = null;
            MetricTypes currentMetricType = MetricTypes.NotSpecified;
            var wasReadingMeasurements = false;
            
            var line = await streamReader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
            {
                return metrics;
            }

            do
            {
                if (line.StartsWith("#"))
                {
                    if (wasReadingMeasurements == true)
                    {
                        // We're done, starting to interpret the next gauge
                        metrics.Add(unfinishedGauge);
                        unfinishedGauge = null;
                    }

                    if (unfinishedGauge == null)
                    {
                        unfinishedGauge = new Gauge();
                    }

                    var metricInfoMatch = Regex.Match(line, MetricInfoRegex);
                    if (metricInfoMatch.Success == false)
                    {
                        throw new Exception("Unable to parse the metric context information");
                    }

                    var scenario = metricInfoMatch.Groups[1].Value;
                    switch (scenario)
                    {
                        case "HELP":
                            unfinishedGauge.Description = metricInfoMatch.Groups[3].Value;
                            break;
                        case "TYPE":
                            unfinishedGauge.Name = metricInfoMatch.Groups[2].Value;
                            currentMetricType = Enum.Parse<MetricTypes>(metricInfoMatch.Groups[3].Value, ignoreCase: true);
                            break;
                    }

                    wasReadingMeasurements = false;
                }
                else
                {
                    if (currentMetricType != MetricTypes.Gauge)
                    {
                        Console.WriteLine("Current metric type is not a gauge, ignoring.");
                    }
                    else
                    {
                        GaugeMeasurement measurement = ParseMeasurement(line);
                        unfinishedGauge.Measurements.Add(measurement);
                    }

                    wasReadingMeasurements = true;
                }

                line = await streamReader.ReadLineAsync();
            } while (rawMetricsStream.Position <= rawMetricsStream.Length && string.IsNullOrWhiteSpace(line) == false);

            // Add the unfinished guage if it wasn't added yet.
            if(metrics.Find(f => f.Name == unfinishedGauge.Name) == null)
            {
                metrics.Add(unfinishedGauge);
            }

            return metrics;
        }

        private static GaugeMeasurement ParseMeasurement(string line)
        {
            var measurement = new GaugeMeasurement();
            var regexOutcome = Regex.Match(line, MeasurementRegex);
            if (regexOutcome.Success == false)
            {
                throw new Exception($"Measurement doesn't follow the required Regex statement ({MeasurementRegex})");
            }

            // Assign value
            measurement.Value = ParseMetricValue(regexOutcome);

            // Get all contextual information
            ParseMetricLabels(regexOutcome, measurement);

            // Assign time, if available
            measurement.Timestamp = ParseMetricTimestamp(regexOutcome);

            return measurement;
        }

        private static double ParseMetricValue(Match regexOutcome)
        {
            if (regexOutcome.Groups.Count < 4 || regexOutcome.Groups[3].Value == null)
            {
                throw new Exception("No metric value was found");
            }

            return double.Parse(regexOutcome.Groups[3].Value);
        }

        private static DateTimeOffset? ParseMetricTimestamp(Match regexOutcome)
        {            
            if (regexOutcome.Groups.Count < 5 || regexOutcome.Groups[4].Value == null)
            {
                return null;
            }

            var unixTimeInSeconds = long.Parse(regexOutcome.Groups[4].Value);
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTimeInSeconds);
        }

        private static void ParseMetricLabels(Match regexOutcome, GaugeMeasurement measurement)
        {
            var labels = regexOutcome.Groups[2].Value;
            
            // Get every individual raw label
            foreach (var rawLabel in labels.Split(','))
            {
                // Split label into information
                var splitLabelInfo = rawLabel.Split('=');

                // Add to the outcome
                measurement.Labels.Add(splitLabelInfo[0], splitLabelInfo[1].Replace("\"", ""));
            }
        }
    }
}
