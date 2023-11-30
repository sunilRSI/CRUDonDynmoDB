using Amazon.SQS.Model;
using EmployeeCatalog.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCatalog.Shared.Providers
{
    public interface ISQSProvider
    {
        Task SendMessagesAsync(string topicName, EmployeeRequest request, CancellationToken cancellationToken);
        Task SendMessageAsync(string topicName, string messageText, TimeSpan? visibilityTimeout = null, TimeSpan? timeToLive = null, CancellationToken cancellationToken = default);
        Task<Message> ReceiveMessageAsync(string topicName, TimeSpan? visibilityTimeout, CancellationToken cancellationToken);
        Task DeleteMessageAsync(string topicName, Message message, CancellationToken cancellationToken);
        Task Initialize();
    }
}
