using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityService.Pages.Account.Manage;

public class ManageNavPages
{
    public static string Index => "Index";

    public static string ChangePassword => "ChangePassword";

    public static string PersonalData => "PersonalData";

    public static string? IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

    public static string? ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

    public static string? PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalData);

    public static string? PageNavClass(ViewContext viewContext, string page)
    {
        var activePage = viewContext.ViewData["ActivePage"] as string
                         ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
    }

    public static string? IndexAriaCurrent(ViewContext viewContext) => AriaCurrent(viewContext, Index);

    public static string? ChangePasswordAriaCurrent(ViewContext viewContext) => AriaCurrent(viewContext, ChangePassword);

    public static string? PersonalDataAriaCurrent(ViewContext viewContext) => AriaCurrent(viewContext, PersonalData);

    public static string? AriaCurrent(ViewContext viewContext, string page)
    {
        var activePage = viewContext.ViewData["ActivePage"] as string
                         ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "page" : null;
    }
}