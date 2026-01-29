using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using consumerMessage.Services.Persistence.Data;

namespace consumerMessage.Services.Consumer.Kafka
{
    public class KafkaConsumer
    {

        private readonly CancellationTokenSource _cancellationTokenSource;

        public KafkaConsumer(ConsumerConfig config)
        {

            _cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                _cancellationTokenSource.Cancel();
            };

        }

        public async Task StartConsuming(string topic, ConsumerConfig config)
        {
            using (var consumer = new ConsumerBuilder<string, string>(config).Build())
            {
                consumer.Subscribe(topic);

                try
                {
                    while (true)
                    {
                        var consumeResult = consumer.Consume(_cancellationTokenSource.Token);
                        Console.WriteLine($"Consumed event from topic '{topic}: key = {consumeResult.Message.Key} value = {consumeResult.Message.Value} '.");
                        DynamoPersistence.SaveProductStockAsync(new Random().Next(1,100), consumeResult.Message.Key).Wait(); //just a random number for demo, but it should come from the message
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl-C was pressed.
                }
                finally
                {
                    consumer.Close();
                }
            
            }
            
        }



    }
}