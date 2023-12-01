using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3.Model;
using Amazon.SQS;
using CRUDonDynmoDB.Middleware;
using EmployeeCatalog.Shared.Data;
using EmployeeCatalog.Shared.Providers;
using EmployeeCatalog.Shared.Services;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace CRUDonDynmoDB
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var awsOptions = builder.Configuration.GetAWSOptions();
            builder.Services.AddDefaultAWSOptions(awsOptions);
            builder.Services.AddAWSService<IAmazonSQS>(); 
            builder.Services.AddAWSService<IAmazonDynamoDB>();

            builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IDbContext, DbContext>();
            builder.Services.AddScoped<ISQSProvider, SQSProvider>();

            //builder.Services.Configure<SQSQueueConfiguration>(builder.Configuration.GetSection("SQS"));

            builder.Services.AddTransient<ExHandlerMiddleware>();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
                await dbContext.Initilize("Employee");
                var sqsProvider = scope.ServiceProvider.GetRequiredService<ISQSProvider>();
                await sqsProvider.Initialize();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseMiddleware<ExHandlerMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}