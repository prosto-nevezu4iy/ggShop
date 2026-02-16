namespace IdentityService.Pages.Account.Login;

public class InputModel
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public bool RememberLogin { get; set; }
    public string ReturnUrl { get; set; }
    public string Button { get; set; }
}
