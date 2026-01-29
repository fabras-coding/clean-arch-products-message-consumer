using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;

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