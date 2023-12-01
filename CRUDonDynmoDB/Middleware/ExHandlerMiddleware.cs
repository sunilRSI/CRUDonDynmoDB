using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace CRUDonDynmoDB.Middleware
{
    public class ExHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<ExHandlerMiddleware> _logger;

        public ExHandlerMiddleware(ILogger<ExHandlerMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
          try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode=(int)HttpStatusCode.InternalServerError;
                ProblemDetails problem = new()
                {
                    Status= (int)HttpStatusCode.InternalServerError,
                    Type= "Server Error",
                    Title= "Server Error",
                    Detail= ex.Message
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
                context.Response.ContentType="application/json";
            }
        }
    }
}
