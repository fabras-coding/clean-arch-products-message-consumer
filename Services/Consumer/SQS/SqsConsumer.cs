using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;


namespace consumerMessage.Services.Consumer.SQS
{
    public class SqsConsumer
    {
        public readonly IAmazonSQS _sqsClient;
        public readonly string _queueUrl;
        public readonly ReceiveMessageRequest _receiveMessageRequest;

        public SqsConsumer(ReceiveMessageRequest receiveMessageRequest, IAmazonSQS sqsClient)
        {
            _receiveMessageRequest = receiveMessageRequest;
            _sqsClient = sqsClient;
            
        }

        public async Task<ReceiveMessageResponse> ReceiveAndDeleteMessagesAsync()
        {
            

            while (true)
            {
                var receiveMessageResponse = await _sqsClient.ReceiveMessageAsync(_receiveMessageRequest);

                if (receiveMessageResponse.Messages.Count == 0)
                {
                    await Task.Delay(1000);
                    // No messages to process
                    continue;
                }

                foreach (var message in receiveMessageResponse.Messages)
                {
                    Console.WriteLine($"Received message: {message.Body}");

                    // Delete the message after processing
                    var deleteRequest = new DeleteMessageRequest
                    {
                        QueueUrl = _receiveMessageRequest.QueueUrl,
                        ReceiptHandle = message.ReceiptHandle
                    };

                    await _sqsClient.DeleteMessageAsync(deleteRequest);
                    Console.WriteLine($"Deleted message with ReceiptHandle: {message.ReceiptHandle}");
                
                }
            }

        }


    }
}