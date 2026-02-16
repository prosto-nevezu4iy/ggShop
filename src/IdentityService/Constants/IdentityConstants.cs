namespace IdentityService.Constants;

public static class IdentityConstants
{
    public const string Username = "USERNAME";
    public const string ResetLink = "RESET_LINK";
    public const string Link = "LINK";
    public const string ApiKey = "api-key";

    public const int AccountConfirmationTemplateId = 1;
    public const int ForgotPasswordTemplateId = 2;

    public const string ConfirmEmailSuccess = "Thank you for confirming your email.";
    public const string ConfirmEmailFailure = "Error confirming your email.";

    public const string AvatarUrlClaimType = "avatar_url";
}