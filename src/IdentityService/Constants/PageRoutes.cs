namespace IdentityService.Constants;

public static class PageRoutes
{
    // Home Pages
    public const string Home = "/Index";

    // Account Pages
    public const string Register = "/Account/Register/Index";
    public const string RegisterConfirmation = "/Account/RegisterConfirmation/Index";
    public const string ConfirmEmail = "/Account/ConfirmEmail/Index";
    public const string ForgotPassword = "/Account/ForgotPassword/Index";
    public const string ForgotPasswordConfirmation = "/Account/ForgotPasswordConfirmation/Index";
    public const string ResetPassword = "/Account/ResetPassword/Index";
    public const string ResetPasswordConfirmation = "/Account/ResetPasswordConfirmation/Index";
    public const string ResendEmailConfirmation = "/Account/ResendEmailConfirmation/Index";
    public const string Logout = "/Account/Logout/Index";
    public const string LoggedOut = "/Account/Logout/LoggedOut";
    public const string Login = "/Account/Login/Index";
    public const string ExternalLogin = "/ExternalLogin/Challenge";
}
