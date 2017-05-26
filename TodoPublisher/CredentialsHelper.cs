using Amazon.Runtime;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TodoPublisher
{
    public class CredentialHelper
    {
        public static BasicAWSCredentials LoadCredentials()
        {
            JObject creds = JObject.Parse(File.ReadAllText("aws-creds.json"));
            return new BasicAWSCredentials(creds["AWSAccessKeyId"].ToString(), creds["AWSSecretKey"].ToString());
        }
    }
}
