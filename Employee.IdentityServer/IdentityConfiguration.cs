using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace Employee.IdentityServer
{
    public class IdentityConfiguration
    {
        public static List<TestUser> TestUsers =>
            new List<TestUser>
            {
        new TestUser
        {
            SubjectId = "1144",
            Username = "sunil",
            Password = "sunil",
            Claims =
            {
                new Claim(JwtClaimTypes.Name, "sunil sunil"),
                new Claim(JwtClaimTypes.GivenName, "sunil"),
                new Claim(JwtClaimTypes.FamilyName, "ghutukade"),
                new Claim(JwtClaimTypes.WebSite, "https://iempower.india.rsystems.com/"),
            }
        }
        };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("myApiScope")
            };
        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                 new ApiResource("myApi")
                    {
                            Scopes = new List<string>{ "myApiScope" },
                            ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) }
                    }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "myclient",
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes = { "myApiScope" },
                    AllowAccessTokensViaBrowser= true 
                }

            };
    }
}
