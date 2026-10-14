using Microsoft.AspNetCore.Identity;

namespace CarService.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = null!;
    public string PreferredLanguage { get; set; } = "uz";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class Roles
{
    public const string Customer = "Customer";
    public const string Mechanic = "Mechanic";
    public const string Admin = "Admin";

    public static readonly string[] All = [Customer, Mechanic, Admin];
}
