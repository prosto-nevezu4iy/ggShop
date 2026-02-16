using System.ComponentModel.DataAnnotations;

namespace IdentityService.Configurations;

public class AuthMessageSenderOptions
{
    [Required]
    public string BrevoKey { get; set; }
}