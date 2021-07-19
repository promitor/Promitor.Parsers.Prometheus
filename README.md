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
