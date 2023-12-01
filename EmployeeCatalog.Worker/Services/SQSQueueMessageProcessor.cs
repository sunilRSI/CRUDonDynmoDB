using Amazon.SQS.Model;
using EmployeeCatalog.Shared.Data;
using Newtonsoft.Json;

namespace EmployeeCatalog.Worker.Services
{
    public class SQSQueueMessageProcessor
    {
        public async Task<bool> ProcessMessageAsync(Message message, CancellationToken cancellationToken = default)
        {
            bool MesaageProcessed = false;
            try
            {
                var queueMessage = JsonConvert.DeserializeObject<EmployeeRequest>(message.Body);
                MesaageProcessed = true;
                //applay Processing logic

            }
            catch (Exception ex)
            {

            }
            return MesaageProcessed;
        }
    }
}
