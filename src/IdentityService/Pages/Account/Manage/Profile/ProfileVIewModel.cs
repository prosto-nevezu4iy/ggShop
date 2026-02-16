using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Pages.Account.Manage.Profile;

public class ProfileVIewModel
{
    public string Username { get; set; }

    public string Email { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string AvatarUrl { get; set; }

    [TempData]
    public string StatusMessage { get; set; }
}