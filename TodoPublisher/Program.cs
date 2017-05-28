using System;
using Amazon.SimpleNotificationService;
using Newtonsoft.Json;

namespace TodoPublisher
{
    class Program
    {
        const string TODO_CREATED_ARN = "arn:aws:sns:ap-southeast-2:599080142044:student-todos";
        static void Main(string[] args)
        {
            var creds = CredentialHelper.LoadCredentials();
            using (var client = new
                AmazonSimpleNotificationServiceClient(creds,
                Amazon.RegionEndpoint.APSoutheast2))
            {
                while (true)
                {
                    Console.WriteLine("Press Enter to create a new todo");
                    var key = Console.ReadKey();

                    if (key.KeyChar == '\r')
                    {
                        var todo = new TodoItem()
                        {
                            CreatedOn = DateTime.UtcNow,
                            Description = $"Test Todo Created on {DateTime.Now.ToString()}",
                            StudentId = new Random().Next(1, 11),
                            Title = $"Todo {new Random().Next(1, 101)}",
                            TodoType = "Self Placement"
                        };
                        var todoSerialized = JsonConvert.SerializeObject(todo, Formatting.Indented);
                        Console.WriteLine("Creating Todo: " + todoSerialized);
                       var res = client.Publish(TODO_CREATED_ARN, todoSerialized);
                    }
                }
            }
        }
    }
}
