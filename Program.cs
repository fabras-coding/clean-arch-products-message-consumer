

// See https://aka.ms/new-console-template for more information
using System;
using Confluent.Kafka;
using Amazon.SQS;
using consumerMessage.Services.Consumer.Kafka;
using Amazon.SQS.Model;
using consumerMessage.Services.Consumer.SQS;

//it servers both topic and SQS queue
const string TOPIQUEUE = "product-created";

Console.WriteLine("Starting consumer application...");

try
{

    var configKafka = new ConsumerConfig
    {
        // User-specific properties that you must set
        BootstrapServers = "localhost:9092",
        SaslUsername = "test",
        SaslPassword = "test",


        // Fixed properties
        SecurityProtocol = SecurityProtocol.Plaintext,
        SaslMechanism = SaslMechanism.Plain,
        GroupId = "kafka-consumer-group-products",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    var kafkaConsumer = new KafkaConsumer(configKafka);

    var sqsClient = new AmazonSQSClient(new AmazonSQSConfig
    {
        ServiceURL = "http://localhost:4566"
    });

    var response = await sqsClient.GetQueueUrlAsync(TOPIQUEUE);

    var receiveMessageRequest = new ReceiveMessageRequest
    {

        QueueUrl = response.QueueUrl,
        MaxNumberOfMessages = 1,
        WaitTimeSeconds = 0,
        MessageAttributeNames = new List<string> { "All" },
        VisibilityTimeout = 30
    };

    var sqsConsumer = new SqsConsumer(receiveMessageRequest, sqsClient);
    Console.WriteLine("Starting SQS consumer...");
    Console.WriteLine("Starting Kafka consumer...");

    var kafkaTask = Task.Run(() => kafkaConsumer.StartConsuming(TOPIQUEUE, configKafka));
    var sqsTask = Task.Run(() => sqsConsumer.ReceiveAndDeleteMessagesAsync());
    Console.WriteLine("Kafka and SQS consumers are running in parallel...");

    await Task.WhenAll(kafkaTask, sqsTask);
    Console.WriteLine("Consumer application has stopped.");

}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}