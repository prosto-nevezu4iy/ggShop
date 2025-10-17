using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new("catalogApp", "Catalog Service Full Access"),
        new("shoppingCartApp", "Shopping Cart Service Full Access")
    ];

    public static IEnumerable<Client> Clients =>
    [
        new()
        {
            ClientId = "postman",
            ClientName = "Postman",
            AllowedScopes = { "openid", "profile", "catalogApp", "shoppingCartApp" },
            RedirectUris = { "https://getpostman.com/oauth2/callback" },
            ClientSecrets = [new Secret("notASecret".Sha256())],
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
        }
    ];
}
