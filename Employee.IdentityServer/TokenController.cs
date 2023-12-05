using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace Employee.IdentityServer
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> GetToken(CancellationToken cancellationToken = default)
        {
            using (var client = new HttpClient())
            {
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = _configuration["TokenUrl"],
                    ClientId = IdentityConfiguration.Clients.FirstOrDefault().ClientId,
                    Scope = IdentityConfiguration.Clients.FirstOrDefault().AllowedScopes.FirstOrDefault(),
                    ClientSecret = "secret",
                });
                if (tokenResponse.IsError)
                {
                    throw new Exception("Token Error");
                }
                return Ok(tokenResponse);
            }

        }
    }
}
