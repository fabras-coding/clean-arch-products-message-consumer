using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace consumerMessage.Services.Persistence.Data
{

    public static class DynamoPersistence
    {

        public static AmazonDynamoDBClient client = new AmazonDynamoDBClient(
            new BasicAWSCredentials("test", "test"),
            new Amazon.DynamoDBv2.AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:4566",
                AuthenticationRegion = "us-east-1"
            }
        );
        public static string tableName = "ProductStock";

        public static async Task SaveProductStockAsync( int stock, string productId)
        {

            productId = productId ?? Guid.NewGuid().ToString();

            Console.WriteLine($"productId='{productId}', stock='{stock}'");
            var request = new PutItemRequest
            {
                TableName = tableName,

                Item = new Dictionary<string, AttributeValue>
                {
                    { "productId", new AttributeValue { S = productId } },
                    { "updatedAt", new AttributeValue { S = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") } },
                    { "stockCount", new AttributeValue { N = stock.ToString() }}
                }
            };
            try
            {
                await client.PutItemAsync(request);
                Console.WriteLine($"Saved product {productId} with stock {stock} to DynamoDB.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving product stock to DynamoDB: {ex.Message}");
            }
        }

    }
}