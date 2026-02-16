using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityService.Pages.Account.Manage;

public static class ManageNavPages
{
    private static string Index => "Index";

    private static string ChangePassword => "ChangePassword";

    public static string IndexAriaCurrent(ViewContext viewContext) => AriaCurrent(viewContext, Index);

    public static string ChangePasswordAriaCurrent(ViewContext viewContext) => AriaCurrent(viewContext, ChangePassword);

    private static string AriaCurrent(ViewContext viewContext, string page)
    {
        var activePage = viewContext.ViewData["ActivePage"] as string
                         ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "page" : null;
    }
}