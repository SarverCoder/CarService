using CarService.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace CarService.Application.Features.Auth.Commands;

public record LoginCommand(string PhoneNumber, string Password) : IRequest<AuthResult>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginCommandHandler(IIdentityService identity) : IRequestHandler<LoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(LoginCommand request, CancellationToken ct) =>
        identity.LoginAsync(request.PhoneNumber, request.Password, ct);
}
