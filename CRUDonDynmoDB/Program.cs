using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Model;
using Amazon.SQS;
using CRUDonDynmoDB.Middleware;
using EmployeeCatalog.Shared.Data;
using EmployeeCatalog.Shared.Providers;
using EmployeeCatalog.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using System.Reflection;
using static System.Net.WebRequestMethods;

namespace CRUDonDynmoDB
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
           // IdentityModelEventSource.ShowPII = true;
            // Add services to the container.
            builder.Services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(s =>
                               {
                                   s.Authority = "https://localhost:7202";
                                   s.Audience = "myApi";
                                   s.RequireHttpsMetadata = false; 
                               });
           
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
            builder.Services.AddScoped<ISQSProvider, SQSProvider>();
            builder.Services.AddTransient<ExHandlerMiddleware>();


            //builder.Services.AddAuthentication("Bearer")
            //        .AddIdentityServerAuthentication("Bearer", options =>
            //            {
            //                options.ApiName = "myApi";
            //                options.Authority = "https://localhost:7202";
            //                options.ApiSecret = "acf2ec6fb01a4b698ba240c2b10a0243";
            //                options.CacheDuration = TimeSpan.FromSeconds(10000);
            //                options.EnableCaching = true;
            //                options.RequireHttpsMetadata = false;
            //                options.SaveToken = true;
            //            });

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "myApi", Version = "v1" });  
                c.AddSecurityDefinition("Bearer",  new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http, 
                    Description = "Please Provide Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    BearerFormat="JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference= new OpenApiReference{Type=ReferenceType.SecurityScheme,Id="Bearer"}
                        },
                        new List<string>{ "myApiScope" }
                    }
                });
            });

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>(); 
                await dbContext.Initilize();
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
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ExHandlerMiddleware>();

            app.MapControllers();


            app.Run();
        }
    }
}