using System;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon;
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TodoProcessor
{
    public class Function
    {
        private string _accessKey;
        private string _secretKey;
        private string _serviceUrl;
        private const string TableName = "student-todos";
        public Function()
        {
            _accessKey = Environment.GetEnvironmentVariable("AccessKey");
            _secretKey = Environment.GetEnvironmentVariable("SecretKey");
            _serviceUrl = Environment.GetEnvironmentVariable("ServiceURL");
        }
        public async Task FunctionHandler(SNSEvent snsEvent)
        {
            var dynamoDbClient = new AmazonDynamoDBClient(
                new BasicAWSCredentials(_accessKey, _secretKey),
                new AmazonDynamoDBConfig
                {
                    ServiceURL = _serviceUrl,
                    RegionEndpoint = RegionEndpoint.APSoutheast2
                });
            await CreateTable(dynamoDbClient, TableName);
            foreach (var record in snsEvent.Records)
            {
                var snsRecord = record.Sns;
                Console.WriteLine($"[{record.EventSource} {snsRecord.Timestamp}] Message = {snsRecord.Message}");

                try
                {
                    var todo = JsonConvert.DeserializeObject<TodoDto>(snsRecord.Message);
                    Console.WriteLine($"Description: {todo.Description} StudentId: {todo.StudentId}");
                    LambdaLogger.Log($"Insert record in the table { TableName}");

                    await dynamoDbClient.PutItemAsync(TableName, new Dictionary<string, AttributeValue>{
                        { "TodoId", new AttributeValue($"{todo.StudentId}-{todo.TodoType}") },
                        { "CreatedOn", new AttributeValue(todo.CreatedOn.ToString()) },
                        { "Title", new AttributeValue(todo.Title.ToString()) },
                        { "Description", new AttributeValue(todo.Description.ToString()) },
                        { "StudentId", new AttributeValue(todo.StudentId.ToString()) },
                        { "TodoType", new AttributeValue(todo.TodoType) }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

        private async Task CreateTable(IAmazonDynamoDB amazonDynamoDBclient, string tableName)
        {
            LambdaLogger.Log(string.Format("Creating {0} Table", tableName));
            var tableCollection = await amazonDynamoDBclient.ListTablesAsync();

            if (!tableCollection.TableNames.Contains(tableName))

                await amazonDynamoDBclient.CreateTableAsync(new CreateTableRequest
                {
                    TableName = tableName,
                    KeySchema = new List<KeySchemaElement> {
                      { new KeySchemaElement { AttributeName="TodoId",  KeyType= KeyType.HASH }},
                        new KeySchemaElement { AttributeName="CreatedOn",  KeyType= KeyType.RANGE }
                    },

                    AttributeDefinitions = new List<AttributeDefinition> {
                        new AttributeDefinition { AttributeName="Title", AttributeType="S" },
                        new AttributeDefinition { AttributeName ="Description",AttributeType="S"},
                        new AttributeDefinition { AttributeName ="StudentId",AttributeType="N"},
                        new AttributeDefinition { AttributeName ="TodoType",AttributeType="S"}
                    },

                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    },
                });
        }
    }

}