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
- Run the following `docker` commands to create Pulsar containers for each required component (example from - https://pulsar.apache.org/docs/4.0.x/deploy-docker/):
  - `ZooKeeper` metadata container
  ```
  docker run -d -p 2181:2181 --net=pulsar \
    -e metadataStoreUrl=zk:zookeeper:2181 \
    -e cluster-name=cluster-a -e managedLedgerDefaultEnsembleSize=1 \
    -e managedLedgerDefaultWriteQuorum=1 \
    -e managedLedgerDefaultAckQuorum=1 \
    -v $(pwd)/data/zookeeper:/pulsar/data/zookeeper \
    --name zookeeper --hostname zookeeper \
    apachepulsar/pulsar-all:latest \
    bash -c "bin/apply-config-from-env.py conf/zookeeper.conf && bin/generate-zookeeper-config.sh conf/zookeeper.conf && exec bin/pulsar zookeeper"
  ```
  - `Cluster metadata initialization` container
  ```
  docker run --net=pulsar \
    --name initialize-pulsar-cluster-metadata \
    apachepulsar/pulsar-all:latest bash -c "bin/pulsar initialize-cluster-metadata \
    --cluster cluster-a \
    --zookeeper zookeeper:2181 \
    --configuration-store zookeeper:2181 \
    --web-service-url http://broker:8080 \
    --broker-service-url pulsar://broker:6650"
  ```
  - `Bookie` container
  ```
  docker run -d -e clusterName=cluster-a \
    -e zkServers=zookeeper:2181 --net=pulsar \
    -e metadataServiceUri=metadata-store:zk:zookeeper:2181 \
    -v $(pwd)/data/bookkeeper:/pulsar/data/bookkeeper \
    --name bookie --hostname bookie \
    apachepulsar/pulsar-all:latest \
    bash -c "bin/apply-config-from-env.py conf/bookkeeper.conf && exec bin/pulsar bookie"
  ```
  - `Broker` container
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

await using var client = PulsarClient.Builder()
    .ServiceUrl(new Uri("pulsar://localhost:6650"))
    .RetryInterval(TimeSpan.FromSeconds(1))
    .Build();
```

## JPM-7: Create Apache Pulsar producer and consumer 
- Create the Pulsar producer and consumer with example send and receive
  - Example producer
  ```
  await using var producer = client.NewProducer()
    .Topic("persistent://public/default/mytopic")
    .Create();

  var tmessage = Encoding.UTF8.GetBytes("Hello, Pulsar!");
  var tmessageId = await producer.Send(tmessage);

  Console.WriteLine($"Message sent with ID: {tmessage}");
  ```

  - Example consumer
  ```
  await using var consumer = client.NewConsumer()
            .Topic("persistent://public/default/mytopic")
            .SubscriptionName("my-subscription")
            .SubscriptionType(SubscriptionType.Exclusive)
            .Create();

  Console.WriteLine("Waiting for messages...");

  var cancellationToken = CancellationToken.None;

  await foreach (var message in consumer.Messages(cancellationToken))
  {
      var content = Encoding.UTF8.GetString(message.Data);
      Console.WriteLine($"Received: {content}");

      // Acknowledge the message
      await consumer.Acknowledge(message, cancellationToken);
  }
  ```

## Running a standalone Pulsar broker
- To run a standalone Pulsar broker, run the following command `pulsar standalone`