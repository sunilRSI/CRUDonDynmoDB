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
        Task SendMessageAsync(string QueueName, EmployeeRequest request, CancellationToken cancellationToken);
        Task<Message> ReceiveMessageAsync(string QueueName, TimeSpan? visibilityTimeout, CancellationToken cancellationToken);
        Task DeleteMessageAsync(string QueueName, Message message, CancellationToken cancellationToken);
        Task Initialize();
    }
}
