using CarService.Application.Features.Auth.Commands;

namespace CarService.Application.Tests;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Valid_command_passes() =>
        Assert.True(_validator.Validate(new RegisterCommand("+998 90 123 45 67", "Ali Valiyev", "secret123")).IsValid);

    [Fact]
    public void Short_password_and_empty_name_fail()
    {
        var result = _validator.Validate(new RegisterCommand("+998901234567", "", "123"));
        Assert.Contains(result.Errors, e => e.PropertyName == "FullName");
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }
}
