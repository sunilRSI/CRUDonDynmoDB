using Amazon.DynamoDBv2;
using Amazon.SQS;
using EmployeeCatalog.Shared.Data;
using EmployeeCatalog.Worker.Services;
using System.Reflection;

namespace EmployeeCatalog.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var awsOptions = builder.Configuration.GetAWSOptions();
            builder.Services.AddDefaultAWSOptions(awsOptions);
            builder.Services.AddAWSService<IAmazonSQS>();
            builder.Services.AddAWSService<IAmazonDynamoDB>();

            builder.Services.AddHostedService<SQSMessageSubscriber>();
            builder.Services.AddTransient<SQSQueueMessageProcessor>();
            builder.Services.AddTransient<SQSQueueMessageListener>();
            //builder.Services.AddTransient<Models.QueueConfiguration>();
            //builder.Services.Configure<Models.SQSQueueConfiguration>(builder.Configuration.GetSection("AWS"));
            var app = builder.Build();
            app.Run();
        }
    }
}