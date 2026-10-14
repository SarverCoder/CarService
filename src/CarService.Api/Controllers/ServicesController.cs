using CarService.Application.Features.Services.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers;

[ApiController]
[Route("api/services")]
public class ServicesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ServiceTypeDto>>> GetAll(CancellationToken ct) =>
        Ok(await sender.Send(new GetServicesQuery(), ct));
}
