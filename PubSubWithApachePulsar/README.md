# `PubSubWithApachePulsar` Example
An example of how to code using a publisher subscriber architecture in Dotnet 8, C# with Apache Pulsar

## Prerequisites
- To install the `Pulsar C# client library`, follow these steps:
  - Create a dotnet project
  - Add the DotPulsar NuGet package via command `dotnet add package DotPulsar` OR via the NuGet Package Manager

## JPM-6: Setup Pulsar Client
- Create the Pulsar client
```
using DotPulsar;

var client = PulsarClient.Builder()
    .ServiceUrl(new Uri("pulsar://localhost:6650"))
    .RetryInterval(TimeSpan.FromSeconds(1))
    .Build();
```