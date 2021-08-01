# Promitor - Prometheus Parser
Provides a parser to translate Prometheus metrics into typed objects.

## Install

Install via NuGet:

```csharp
Install-Package Promitor.Parsers.Prometheus.Core
```

## Features

Easily parse raw metrics from a stream to concrete types:

```csharp
var metrics = await PrometheusMetricsParser.ParseAsync(rawMetricsStream);
```

Easily parse raw metrics from an HTTP response to concrete types:

```csharp
HttpResponseMessage httpResponse = await httpClient.GetAsync("/scrape");
var metrics = await httpResponse.ReadAsPrometheusMetricsAsync(rawMetricsStream);
```
