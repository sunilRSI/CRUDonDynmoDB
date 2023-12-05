using IdentityServer4.Models;
using IdentityServer4.Test;
namespace Employee.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddIdentityServer()
                            .AddInMemoryClients(IdentityConfiguration.Clients)
                            .AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
                             .AddInMemoryApiResources(IdentityConfiguration.ApiResources)
                            .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
                            .AddTestUsers(IdentityConfiguration.TestUsers)
                            .AddDeveloperSigningCredential();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapControllers();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRouting();
            app.UseIdentityServer();

            app.Run();
        }
    }
}