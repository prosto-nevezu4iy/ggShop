namespace Common.Infrastructure.Authorization;

public class PermissionsList
{
    public const string CatalogRead = "catalog.read";
    public const string CatalogCreate = "catalog.create";
    public const string CatalogUpdate = "catalog.update";
    public const string CatalogDelete = "catalog.delete";

    public const string ShoppingCartRead = "shoppingcart.read";
    public const string ShoppingCartCreate = "shoppingcart.create";
    public const string ShoppingCartUpdate = "shoppingcart.update";
    public const string ShoppingCartDelete = "shoppingcart.delete";

    public const string OrderRead = "order.read";
    public const string OrderCreate = "order.create";
    public const string OrderUpdate = "order.update";
    public const string OrderDelete = "order.delete";

    public static IEnumerable<string> All => new List<string>
    {
        CatalogRead,
        CatalogCreate,
        CatalogUpdate,
        CatalogDelete,
        ShoppingCartRead,
        ShoppingCartCreate,
        ShoppingCartUpdate,
        ShoppingCartDelete,
        OrderRead,
        OrderCreate,
        OrderUpdate,
        OrderDelete
    };

    public static IEnumerable<string> UserPermissions => new List<string>
    {
        CatalogRead,
        ShoppingCartRead,
        ShoppingCartCreate,
        ShoppingCartUpdate,
        ShoppingCartDelete,
        OrderRead,
        OrderCreate
    };
}