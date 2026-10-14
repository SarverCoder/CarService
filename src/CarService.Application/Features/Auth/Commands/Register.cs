using CarService.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace CarService.Application.Features.Auth.Commands;

public record RegisterCommand(string PhoneNumber, string FullName, string Password) : IRequest<AuthResult>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\+?[\d\s\-()]{9,17}$")
            .WithMessage("Telefon raqami noto'g'ri.");
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public class RegisterCommandHandler(IIdentityService identity) : IRequestHandler<RegisterCommand, AuthResult>
{
    public Task<AuthResult> Handle(RegisterCommand request, CancellationToken ct) =>
        identity.RegisterAsync(request.PhoneNumber, request.FullName.Trim(), request.Password, ct);
}
