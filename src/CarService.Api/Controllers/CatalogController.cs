using CarService.Application.Features.Catalog.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController(ISender sender) : ControllerBase
{
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<CarBrandDto>>> GetBrands(CancellationToken ct) =>
        Ok(await sender.Send(new GetCarBrandsQuery(), ct));

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(CancellationToken ct) =>
        Ok(await sender.Send(new GetCategoriesQuery(), ct));

    [HttpGet("vin/{vin}")]
    public async Task<ActionResult<VinDecodeResult>> DecodeVin(string vin, CancellationToken ct) =>
        Ok(await sender.Send(new DecodeVinQuery(vin), ct));
}
