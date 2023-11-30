using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCatalog.Shared.Data
{
    public class SQSQueueConfiguration
    {
        public string AwsAccessKey { get; set; }
        public string AwsSecretKey { get; set; }
        public string AwsRegion { get; set; }
        public string QueueName { get; set; }
        public string QueueUrl { get; set; }
    }
}
