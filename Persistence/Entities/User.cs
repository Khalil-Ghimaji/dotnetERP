using Microsoft.AspNetCore.Identity;

namespace Persistence.Entities;

public class User:IdentityUser
{
    public string? firstName { get; set; }
    public string? lastName { get; set; }
}