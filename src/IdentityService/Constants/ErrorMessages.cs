namespace IdentityService.Constants;

public class ErrorMessages
{
    public const string UserNotFoundWithId = "Unable to load user with ID {0}.";
    public const string UserNotFoundWithEmail = "Unable to load user with email '{0}'.";

    public const string MustProvideCode = "A code must be supplied for password reset.";
}