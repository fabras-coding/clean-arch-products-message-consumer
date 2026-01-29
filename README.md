# Consumer Message Application

A .NET 9.0 console application that consumes messages from both **Apache Kafka** and **AWS SQS** queues in parallel.

## Overview

This application demonstrates a dual-consumer pattern, simultaneously consuming messages from a Kafka topic and an SQS queue. It's designed to handle product creation events from multiple messaging systems, making it ideal for distributed architectures that leverage multiple messaging platforms.

## Features

- **Dual Message Consumption**: Processes messages from both Kafka and SQS in parallel
- **LocalStack Integration**: Uses LocalStack for local AWS SQS/DynamoDB development
- **Error Handling**: Includes exception handling and logging for production readiness
- **Configurable**: Easy-to-modify consumer configurations for different environments

## Architecture

```
┌─────────────────────────────────────────┐
│      Consumer Application               │
│  (Runs Kafka & SQS Consumers in         │
│   parallel using Task.WhenAll)          │
├─────────────────────────────────────────┤
│  Kafka Consumer         │  SQS Consumer │
│  (localhost:9092)       │  (LocalStack) │
└─────────────────────────────────────────┘
```

## Prerequisites

- **.NET 9.0** SDK or later
- **Docker** and **Docker Compose** (for LocalStack infrastructure)
- **Apache Kafka** (or LocalStack for local development)
- Basic knowledge of message queues and async/await patterns

## Project Structure

```
consumerMessage/
├── Program.cs                          # Application entry point
├── consumerMessage.csproj             # Project configuration
├── consumerMessage.sln                # Solution file
├── Services/
│   └── Consumer/
│       ├── Kafka/
│       │   └── KafkaConsumer.cs       # Kafka consumer implementation
│       └── SQS/
│           └── SqsConsumer.cs         # SQS consumer implementation
└── infra/
    └── localstack/
        └── dynamoDb/
            └── docker-compose.yml     # LocalStack infrastructure setup
```

## Installation

### 1. Clone the Repository
```bash
git clone <repository-url>
cd consumerMessage
```

### 2. Install Dependencies
The NuGet packages are configured in `consumerMessage.csproj`:
- `Confluent.Kafka` - For Kafka message consumption
- `AWSSDK.SQS` - For AWS SQS integration

```bash
dotnet restore
```

### 3. Start Infrastructure (LocalStack)
```bash
cd infra/localstack/dynamoDb
docker-compose up -d
```

This starts:
- **LocalStack** on `http://localhost:4566` (SQS endpoint)
- **DynamoDB** local support

### 4. Start Kafka (if not running)
Ensure Kafka is running on `localhost:9092`. If using Docker:
```bash
docker run -d --name kafka -e KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://localhost:9092 \
  -e KAFKA_ZOOKEEPER_CONNECT=localhost:2181 confluentinc/cp-kafka
```

## Configuration

### Kafka Configuration (Program.cs)

```csharp
var configKafka = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    SaslUsername = "test",
    SaslPassword = "test",
    SecurityProtocol = SecurityProtocol.Plaintext,
    SaslMechanism = SaslMechanism.Plain,
    GroupId = "kafka-consumer-group-products",
    AutoOffsetReset = AutoOffsetReset.Earliest
};
```

### SQS Configuration (Program.cs)

```csharp
var sqsClient = new AmazonSQSClient(new AmazonSQSConfig
{
    ServiceURL = "http://localhost:4566"  // LocalStack endpoint
});
```

### Topic/Queue Name
Both consumers listen to: **`product-created`**

## Running the Application

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

### Expected Output
```
Starting consumer application...
Starting SQS consumer...
Starting Kafka consumer...
Kafka and SQS consumers are running in parallel...
```

The application will continue running and consuming messages until interrupted (Ctrl+C).

## How It Works

1. **Initialization**: Application configures both Kafka and SQS clients with their respective settings
2. **Consumer Creation**: `KafkaConsumer` and `SqsConsumer` instances are instantiated
3. **Parallel Execution**: Both consumers run concurrently using `Task.Run()`
4. **Synchronization**: `Task.WhenAll()` waits for both consumers to complete
5. **Message Processing**: Each consumer handles incoming messages from its respective source

## NuGet Dependencies

- **Confluent.Kafka** - Apache Kafka client library
- **AWSSDK.SQS** - AWS SDK for SQS operations

## Development

### Adding New Features

1. **Custom Message Processing**: Modify `KafkaConsumer.cs` or `SqsConsumer.cs`
2. **Additional Topics/Queues**: Add new instances in `Program.cs` and run them in parallel
3. **Persistence**: Implement DynamoDB storage using the LocalStack integration

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Connection refused (Kafka) | Ensure Kafka is running on `localhost:9092` |
| SQS queue not found | Verify LocalStack is running and queue exists |
| Consumer hangs | Check network connectivity and firewall settings |
| SASL authentication fails | Verify Kafka SASL credentials match configuration |

## Roadmap
- [ ] Increase products count stock in a DynamoDB Table

