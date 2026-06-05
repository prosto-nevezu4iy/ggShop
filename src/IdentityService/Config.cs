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
            ClientId = "scalar",
            ClientName = "Scalar",
            AllowedScopes = { "openid", "profile", "catalogApp", "shoppingCartApp" },
            RedirectUris = { "http://localhost:7001/scalar/", "http://localhost:7002/scalar/" },
            RequireClientSecret = false,
            RequirePkce = true,
            AllowedGrantTypes = GrantTypes.Code,
            AllowAccessTokensViaBrowser = true,
            AllowedCorsOrigins =
            {
                "http://localhost:7001",
                "http://localhost:7002"
            }
        }
    ];
}
