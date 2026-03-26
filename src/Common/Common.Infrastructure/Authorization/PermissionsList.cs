namespace Common.Infrastructure.Authorization;

public class PermissionsList
{
    public const string CatalogCreate = "catalog:create";
    public const string CatalogUpdate = "catalog:update";
    public const string CatalogDelete = "catalog:delete";

    public const string OrderRead = "order:read";
    public const string OrderCreate = "order:create";
    public const string OrderUpdate = "order:update";
    public const string OrderDelete = "order:delete";

    public static IEnumerable<string> All => new List<string>
    {
        CatalogCreate,
        CatalogUpdate,
        CatalogDelete,
        OrderRead,
        OrderCreate,
        OrderUpdate,
        OrderDelete
    };

    public static IEnumerable<string> UserPermissions => new List<string>
    {
        OrderRead,
        OrderCreate
    };
}