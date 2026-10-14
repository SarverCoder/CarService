namespace CarService.Infrastructure.Identity;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "CarService";
    public string Audience { get; set; } = "CarService.Client";
    public string Key { get; set; } = null!;
    public int ExpiresMinutes { get; set; } = 60 * 24;
}
