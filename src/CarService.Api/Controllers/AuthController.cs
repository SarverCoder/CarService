using CarService.Application.Features.Auth.Commands;
using CarService.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResult>> Register(RegisterCommand command, CancellationToken ct) =>
        Ok(await sender.Send(command, ct));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResult>> Login(LoginCommand command, CancellationToken ct) =>
        Ok(await sender.Send(command, ct));

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfile>> Me([FromServices] ICurrentUser currentUser, [FromServices] IIdentityService identity, CancellationToken ct)
    {
        var profile = currentUser.UserId is { } id ? await identity.GetProfileAsync(id, ct) : null;
        return profile is null ? Unauthorized() : Ok(profile);
    }
}
