using EmployeeCatalog.Shared.Data;
using EmployeeCatalog.Worker.Services;

namespace EmployeeCatalog.Worker
{
    public class SQSMessageSubscriber : IHostedService
    {
        private readonly ILogger<SQSMessageSubscriber> logger;
        private readonly SQSQueueMessageListener _sQSQueueMessageListener;
        private readonly CancellationTokenSource _cancellationTokenSource;
        public SQSMessageSubscriber(ILogger<SQSMessageSubscriber> logger, SQSQueueMessageListener sQSQueueMessageListener)
        {
            this.logger = logger;
            _sQSQueueMessageListener = sQSQueueMessageListener;
            _cancellationTokenSource = new CancellationTokenSource();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Backgroud service started");
            cancellationToken = _cancellationTokenSource.Token;
            foreach (var queueName in EmployeeSQSQueueName.AllSQSQueueNames)
            {
                _ = Task.Factory.StartNew(async () => await _sQSQueueMessageListener.StartListenAsync(queueName, cancellationToken), TaskCreationOptions.LongRunning);
                // _ = Task.Factory.StartNew(async () => await queueListener.SendMessageAsync(cancellationToken), TaskCreationOptions.LongRunning);
            }
            return Task.CompletedTask;
        } 
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
