using CarService.Application.Common.Exceptions;
using CarService.Application.Interfaces;
using CarService.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace CarService.Infrastructure.Identity;

public class IdentityService(UserManager<ApplicationUser> userManager, JwtTokenService tokens) : IIdentityService
{
    public async Task<AuthResult> RegisterAsync(string phoneNumber, string fullName, string password, CancellationToken ct)
    {
        var phone = PhoneNumber.Create(phoneNumber).Value;
        if (await userManager.FindByNameAsync(phone) is not null)
            throw new ConflictException("Bu telefon raqami bilan foydalanuvchi allaqachon ro'yxatdan o'tgan.");

        var user = new ApplicationUser { UserName = phone, PhoneNumber = phone, FullName = fullName };
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["Password"] = result.Errors.Select(e => e.Description).ToArray()
            });

        await userManager.AddToRoleAsync(user, Roles.Customer);
        return await BuildResultAsync(user);
    }

    public async Task<AuthResult> LoginAsync(string phoneNumber, string password, CancellationToken ct)
    {
        var phone = PhoneNumber.Create(phoneNumber).Value;
        var user = await userManager.FindByNameAsync(phone);
        if (user is null || !await userManager.CheckPasswordAsync(user, password))
            throw new UnauthorizedException("Telefon raqami yoki parol noto'g'ri.");

        return await BuildResultAsync(user);
    }

    public async Task<UserProfile?> GetProfileAsync(Guid userId, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return null;
        var roles = await userManager.GetRolesAsync(user);
        return new UserProfile(user.Id, user.FullName, user.PhoneNumber ?? string.Empty, roles.ToList());
    }

    private async Task<AuthResult> BuildResultAsync(ApplicationUser user)
    {
        var roles = (await userManager.GetRolesAsync(user)).ToList();
        var (token, expiresAt) = tokens.CreateToken(user, roles);
        return new AuthResult(user.Id, user.FullName, user.PhoneNumber ?? string.Empty, roles, token, expiresAt);
    }
}
