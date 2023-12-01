using Amazon;
using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.Model;
using EmployeeCatalog.Shared.Data;
using Microsoft.Extensions.Logging;
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
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger<SQSProvider> _logger;

        public SQSProvider(IAmazonSQS sqsClient, ILogger<SQSProvider> logger)
        {
            _sqsClient = sqsClient;
            _logger = logger;
        }
      
        /// <summary>
        /// Initialize: This method will get called in program.cs
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            foreach (var QueueName in EmployeeSQSQueueName.AllSQSQueueNames)
            {
                bool isQueueExistAsync = await IsQueueExistAsync(QueueName);
                if (!isQueueExistAsync)
                {
                    CreateQueueRequest createQueueRequest = new CreateQueueRequest
                    {
                        QueueName = QueueName,

                    };
                    CreateQueueResponse createQueueResponse = await _sqsClient.CreateQueueAsync(createQueueRequest);
                }
            }
        } 
        public Task DeleteMessageAsync(string QueueName, Message message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Message> ReceiveMessageAsync(string QueueName, TimeSpan? visibilityTimeout, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        } 
        public async Task SendMessageAsync(string QueueName, EmployeeRequest request, CancellationToken cancellationToken)
        {
            GetQueueUrlResponse getQueueUrlResponse = await _sqsClient.GetQueueUrlAsync(QueueName, cancellationToken);
            SendMessageRequest sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = getQueueUrlResponse.QueueUrl,
                MessageBody = JsonSerializer.Serialize(request),
            };
            SendMessageResponse messageResponse = await _sqsClient.SendMessageAsync(sendMessageRequest, cancellationToken);
        }
        private async Task<bool> IsQueueExistAsync(string QueueName, CancellationToken cancellationToken = default)
        {
            try
            {
                GetQueueUrlResponse getQueueUrlResponse = await _sqsClient.GetQueueUrlAsync(QueueName, cancellationToken);
            }
            catch (QueueDoesNotExistException)
            {
                _logger.LogInformation($"QueueName {QueueName} not exist");
                return false;
            }
            return true;
        }

    }

}
