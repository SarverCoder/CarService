using CarService.Application.Features.Catalog.Commands;
using CarService.Application.Features.Catalog.Queries;
using CarService.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers;

[ApiController]
[Route("api/parts")]
public class PartsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<PartListItemDto>>> Search([FromQuery] SearchPartsQuery query, CancellationToken ct) =>
        Ok(await sender.Send(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PartDetailsDto>> GetById(Guid id, CancellationToken ct) =>
        Ok(await sender.Send(new GetPartByIdQuery(id), ct));

    /// <summary>CSV fayldan qismlarni import qilish (faqat admin).</summary>
    [Authorize(Roles = Roles.Admin)]
    [HttpPost("import")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<ImportPartsResult>> Import(IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        return Ok(await sender.Send(new ImportPartsCommand(stream), ct));
    }
}
