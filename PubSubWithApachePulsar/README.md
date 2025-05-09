# `PubSubWithApachePulsar` Example
An example of how to code using a publisher subscriber architecture in Dotnet 8, C# with Apache Pulsar

## Prerequisites
- Download and install `Docker Desktop` to run a standalone Pulsar broker in Docker
- Download and install a `Java JDK` (we are installing Java SE `JDK 17.0.12`) - https://www.oracle.com/java/technologies/javase/jdk17-archive-downloads.html
  - Add system path to Java bin folder e.g `D:\Program Files\Java\jdk-17\bin`
  - Verify that the installation was successful by running command `java -version`
- Download a `standalone Pulsar cluster` locally for local testing - https://pulsar.apache.org/docs/4.0.x/getting-started-standalone/
  - Add system path to Pulsar bin folder e.g `D:\apache-pulsar-4.0.4\bin`
  - Verify that the installation was successful by running command `pulsar --help`
- To install the `Pulsar C# client library`, follow these steps:
  - Create a dotnet project
  - Add the DotPulsar NuGet package via command `dotnet add package DotPulsar` OR via the NuGet Package Manager
- Run the following `docker` command to create a Pulsar broker (example from - https://pulsar.apache.org/docs/4.0.x/deploy-docker/):
```
docker run -d -p 6650:6650 -p 8080:8080 --net=pulsar \
  -e metadataStoreUrl=zk:zookeeper:2181 \
  -e zookeeperServers=zookeeper:2181 \
  -e clusterName=cluster-a \
  -e managedLedgerDefaultEnsembleSize=1 \
  -e managedLedgerDefaultWriteQuorum=1 \
  -e managedLedgerDefaultAckQuorum=1 \
  --name broker --hostname broker \
  apachepulsar/pulsar-all:latest \
  bash -c "bin/apply-config-from-env.py conf/broker.conf && exec bin/pulsar broker"
```

## JPM-6: Setup Pulsar Client
- Create the Pulsar client
```
using DotPulsar;

var client = PulsarClient.Builder()
    .ServiceUrl(new Uri("pulsar://localhost:6650"))
    .RetryInterval(TimeSpan.FromSeconds(1))
    .Build();
```

## JPM-7: Create Apache Pulsar producer and consumer 
- Create the Pulsar producer and consumer with example send and receive