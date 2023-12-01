using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EmployeeCatalog.Worker.Services
{
    public class SQSQueueMessageListener
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger<SQSQueueMessageListener> _logger;
        private readonly SQSQueueMessageProcessor _sQSQueueMessageProcessor;
        string _queueName=string.Empty;

        public SQSQueueMessageListener(IAmazonSQS sqsClient, ILogger<SQSQueueMessageListener> logger, SQSQueueMessageProcessor sQSQueueMessageProcessor)
        {
            _sqsClient = sqsClient;
            _logger = logger;
            _sQSQueueMessageProcessor = sQSQueueMessageProcessor;
        }
        public async ValueTask StartListenAsync(string QueueName, CancellationToken cancellationToken = default)
        {
            string queueUrl = await GetQueueUrlAsync(QueueName, cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest()
                    {
                        QueueUrl = queueUrl,
                        MaxNumberOfMessages = 10,
                    };
                    _queueName = QueueName;
                    var readRespone = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);
                    if (readRespone.Messages != null && readRespone.Messages.Count > 0)
                    {
                        await Parallel.ForEachAsync(readRespone.Messages, SendMessageForProcessing);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is AmazonSQSException amazonex &&
                        string.Equals(amazonex.ErrorCode, "AWS.SimpleQueueService.NonExistentQueue", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _logger.LogError($"Failed to read message from queue either Invalid queue url or queue doesn't exist. Error: {ex.Message}", ex);

                    }
                    _logger.LogError($"Error while processing message from {QueueName}: {ex.Message}", ex);
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        }

        public async Task CreateQueue(string QueueName, CancellationToken cancellationToken = default)
        {
            string queueUrl = await GetQueueUrlAsync(QueueName, cancellationToken);
            if (string.IsNullOrWhiteSpace(queueUrl))
            {
                CreateQueueRequest createQueueRequest = new CreateQueueRequest
                {
                    QueueName = QueueName,
                };
                CreateQueueResponse createQueueResponse = await this._sqsClient.CreateQueueAsync(createQueueRequest, cancellationToken);
            }
        }
        public async Task SendMessageAsync(string QueueName, CancellationToken cancellationToken = default)
        {
            string queueUrl = await GetQueueUrlAsync(QueueName, cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    SendMessageRequest sendMessageRequest = new SendMessageRequest
                    {
                        QueueUrl = queueUrl,
                        MessageBody = JsonSerializer.Serialize($"hiiiiiii at {DateTime.UtcNow}"),
                    };
                    SendMessageResponse messageResponse = await _sqsClient.SendMessageAsync(sendMessageRequest);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        }
        private async ValueTask SendMessageForProcessing(Message message, CancellationToken cancellationToken = default)
        {
            string queueUrl = await GetQueueUrlAsync(_queueName, cancellationToken);
            bool MesaageProcessed = await _sQSQueueMessageProcessor.ProcessMessageAsync(message, cancellationToken);
            if (MesaageProcessed)
            {
                var deleteRequest = new DeleteMessageRequest
                {
                    QueueUrl = queueUrl,
                    ReceiptHandle = message.ReceiptHandle
                };

                await _sqsClient.DeleteMessageAsync(deleteRequest, cancellationToken);
            }
        }
        private async Task<string> GetQueueUrlAsync(string QueueName, CancellationToken cancellationToken)
        {
            string queueUrl = string.Empty;
            try
            {
                GetQueueUrlResponse getQueueUrlResponse = await this._sqsClient.GetQueueUrlAsync(QueueName, cancellationToken);
                queueUrl = getQueueUrlResponse.QueueUrl;
            }
            catch (QueueDoesNotExistException)
            {
                return queueUrl;
            }
            return queueUrl;
        }

    }
}
