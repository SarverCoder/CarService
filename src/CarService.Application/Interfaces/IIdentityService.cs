namespace CarService.Application.Interfaces;

public record AuthResult(Guid UserId, string FullName, string PhoneNumber, IReadOnlyList<string> Roles, string AccessToken, DateTime ExpiresAt);

public record UserProfile(Guid UserId, string FullName, string PhoneNumber, IReadOnlyList<string> Roles);

public interface IIdentityService
{
    Task<AuthResult> RegisterAsync(string phoneNumber, string fullName, string password, CancellationToken ct);
    Task<AuthResult> LoginAsync(string phoneNumber, string password, CancellationToken ct);
    Task<UserProfile?> GetProfileAsync(Guid userId, CancellationToken ct);
}
