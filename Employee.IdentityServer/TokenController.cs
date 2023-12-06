using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
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
        public async Task<IActionResult> GetToken([Required] string UserId, [Required] string Password, CancellationToken cancellationToken = default)
        {
            TestUser testUser = IdentityConfiguration.TestUsers.FirstOrDefault(x => x.Username == UserId && x.Password == Password);
            if (testUser == null)
            {
                return BadRequest("Invalid Username and password");
            }
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
