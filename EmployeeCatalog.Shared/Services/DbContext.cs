using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using EmployeeCatalog.Shared.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Amazon.Runtime.Internal;
using Microsoft.Extensions.Configuration;

namespace EmployeeCatalog.Shared.Services
{
    public class DbContext : IDbContext
    {
        private readonly ILogger<DbContext> _logger;
        private readonly IAmazonDynamoDB _client;
        private readonly IConfiguration _configuration;
        private readonly string _tableName=string.Empty;

        public DbContext(ILogger<DbContext> logger, IAmazonDynamoDB amazonDynamoDBClient, IConfiguration configuration)
        {
            _logger = logger;
            _client = amazonDynamoDBClient;
            _configuration = configuration;
            _tableName = _configuration["dynamoDBTableName"];
        }
        public async Task Initilize()
        {
            await CreateTable(_tableName);

        }
        public async Task CreateTable(string TableName)
        {
            Employee emp;
            try
            {
                bool tableExist = await isTableExistAsync(TableName);
                if (!tableExist)
                {
                    var request = new CreateTableRequest
                    {
                        TableName = TableName,
                        AttributeDefinitions = new List<AttributeDefinition>()
                        {
                            new AttributeDefinition
                            {
                                AttributeName = nameof(emp.Id),
                                AttributeType = ScalarAttributeType.S
                            },
                            // new AttributeDefinition
                            //{
                            //    AttributeName = nameof(emp.Name),
                            //    AttributeType = ScalarAttributeType.S
                            //},
                        },
                        KeySchema = new List<KeySchemaElement>()
                        {
                             new KeySchemaElement
                                {
                                    AttributeName = nameof(emp.Id),
                                    KeyType =KeyType.HASH
                                },
                             //new KeySchemaElement
                             //   {
                             //       AttributeName = nameof(emp.Name),
                             //       KeyType =KeyType.RANGE
                             //   },
                        },
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 10,
                            WriteCapacityUnits = 5
                        }
                    };
                    var createResponse = await _client.CreateTableAsync(request);
                    while (true)
                    {
                        if (_client.DescribeTableAsync(TableName).GetAwaiter().GetResult().Table.TableStatus == TableStatus.ACTIVE)
                        {
                            break;
                        }
                    }
                    var item = new Dictionary<string, AttributeValue>
                    {
                        [nameof(emp.Id)] = new AttributeValue { S = Guid.NewGuid().ToString() },
                        [nameof(emp.Name)] = new AttributeValue { S = "Default" },
                        [nameof(emp.Designation)] = new AttributeValue { S = "Default" },
                        [nameof(emp.Age)] = new AttributeValue { N = "0" }
                    };

                    var request2 = new PutItemRequest
                    {
                        TableName = TableName,
                        Item = item,
                    };

                    var putResponse = await _client.PutItemAsync(request2);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;

            }
        }
        public async Task<bool> isTableExistAsync(string TableName)
        {
            try
            {
                var result = await _client.DescribeTableAsync(TableName);
            }
            catch (ResourceNotFoundException)
            {
                return false;
            }
            return true;
        }
    }
}
