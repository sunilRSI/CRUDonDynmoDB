using Amazon;
using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.Model;
using EmployeeCatalog.Shared.Data;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeCatalog.Shared.Providers
{
    public class SQSProvider : ISQSProvider
    {
        private readonly IOptions<SQSQueueConfiguration> _options; 
        public SQSProvider(IOptions<SQSQueueConfiguration> options)
        { 
            _options = options;
        }
        public async Task<bool> IsQueueExistAsync(AmazonSQSClient sQSClient, string QueueName, CancellationToken cancellationToken = default)
        {
            try
            {

                GetQueueUrlResponse getQueueUrlResponse = await sQSClient.GetQueueUrlAsync(QueueName, cancellationToken);
            }
            catch (QueueDoesNotExistException)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Initialize: This method will get called in program.cs
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            var sQSClient = FetchAmazonSQSClient();
            foreach (var topicName in EmployeeSQSQueueTopic.AllTopics)
            {
                bool isQueueExistAsync = await IsQueueExistAsync(sQSClient, topicName);
                if (!isQueueExistAsync)
                {
                    CreateQueueRequest createQueueRequest = new CreateQueueRequest
                    {
                        QueueName = topicName,

                    };
                    CreateQueueResponse createQueueResponse = await sQSClient.CreateQueueAsync(createQueueRequest); 
                } 
            }  
        }

        public Task DeleteMessageAsync(string topicName, Message message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Message> ReceiveMessageAsync(string topicName, TimeSpan? visibilityTimeout, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SendMessageAsync(string topicName, string messageText, TimeSpan? visibilityTimeout = null, TimeSpan? timeToLive = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task SendMessagesAsync(string topicName, EmployeeRequest request, CancellationToken cancellationToken)
        {
            AmazonSQSClient sqsclient = FetchAmazonSQSClient();// FetchSQSClient(topicName).Client;
            GetQueueUrlResponse getQueueUrlResponse = await sqsclient.GetQueueUrlAsync(topicName, cancellationToken);
            SendMessageRequest sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = getQueueUrlResponse.QueueUrl,
                MessageBody = JsonSerializer.Serialize(request),
            };
            SendMessageResponse messageResponse = await sqsclient.SendMessageAsync(sendMessageRequest, cancellationToken);
        } 
        private AmazonSQSClient FetchAmazonSQSClient()
        {
            SQSQueueConfiguration sQSQueueConfiguration = _options.Value;
            RegionEndpoint regionEndpoint = RegionEndpoint.GetBySystemName(sQSQueueConfiguration.AwsRegion);
           return new AmazonSQSClient(sQSQueueConfiguration.AwsAccessKey, sQSQueueConfiguration.AwsSecretKey, regionEndpoint);

        } 
    }
     
}
