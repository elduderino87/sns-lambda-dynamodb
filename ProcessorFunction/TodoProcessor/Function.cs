using System;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TodoProcessor
{
    public class Function
    {
        public void FunctionHandler(SNSEvent snsEvent)
        {
            foreach (var record in snsEvent.Records)
            {
                var snsRecord = record.Sns;
                Console.WriteLine($"[{record.EventSource} {snsRecord.Timestamp}] Message = {snsRecord.Message}");

                try
                {
                    var todo = JsonConvert.DeserializeObject<TodoDto>(snsRecord.Message);
                    Console.WriteLine($"Description: {todo.Description} StudentId: {todo.StudentId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }
    }

}
