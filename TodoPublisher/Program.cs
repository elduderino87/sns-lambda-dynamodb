using Amazon.Runtime;
using System;
using Amazon.SimpleNotificationService;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TodoPublisher;

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
                            Description = $"Test Todo Created on {DateTime.Now.ToString()}",
                            StudentId = 19,
                            Title = $"Todo 1",
                            TodoType = "Self Placement"
                        };
                        var todoSerialized = JsonConvert.SerializeObject(todo, Formatting.Indented);
                        Console.WriteLine("Creating Todo: " + todoSerialized);
                        client.Publish(TODO_CREATED_ARN, todoSerialized);
                    }
                }
            }
        }
    }
}
