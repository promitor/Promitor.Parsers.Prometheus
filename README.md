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

## Support

Learn more about our support options [here](https://github.com/tomkerkhove/promitor/blob/master/SUPPORT.md).

Thanks for those who are supporting us via [GitHub Sponsors](https://github.com/sponsors/tomkerkhove/).

[![Carlo Garcia-Mier](https://raw.githubusercontent.com/tomkerkhove/promitor/master//media/supporters/CarloGarcia.jpg)](https://github.com/CarloGarcia)
[![Jorge Turrado Ferrero](https://raw.githubusercontent.com/tomkerkhove/promitor/master//media/supporters/JorTurFer.jpg)](https://github.com/JorTurFer)
[![Karl Ots](https://raw.githubusercontent.com/tomkerkhove/promitor/master//media/supporters/karlgots.jpg)](https://github.com/karlgots)
[![Loc Mai](https://raw.githubusercontent.com/tomkerkhove/promitor/master//media/supporters/locmai.jpg)](https://github.com/locmai)
[![Lovelace Engineering](https://raw.githubusercontent.com/tomkerkhove/promitor/master//media/supporters/LovelaceEngineering.png)](https://github.com/LovelaceEngineering)
[![Nills Franssens](https://raw.githubusercontent.com/tomkerkhove/promitor/master//media/supporters/nillsf.jpg)](https://github.com/NillsF)
[![Richard Simpson](https://raw.githubusercontent.com/tomkerkhove/promitor/master//media/supporters/RichiCoder1.jpg)](https://github.com/RichiCoder1)
[![Sam Vanhoutte](https://raw.githubusercontent.com/tomkerkhove/promitor/master//media/supporters/samvanhoutte.png)](https://github.com/samvanhoutte)

## Donate

Promitor is fully OSS and built free-of-charge, however, if you appreciate my work
you can do a small donation.

[![Donate](https://img.shields.io/badge/Donate%20via-GitHub-blue.svg?style=flat-square)](https://github.com/sponsors/promitor)
