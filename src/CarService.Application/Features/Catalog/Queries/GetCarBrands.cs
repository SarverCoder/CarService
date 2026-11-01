using CarService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarService.Application.Features.Catalog.Queries;

public record CarModelDto(Guid Id, string Name, int YearFrom, int? YearTo);

public record CarBrandDto(Guid Id, string Name, IReadOnlyList<CarModelDto> Models);

public record GetCarBrandsQuery : IRequest<IReadOnlyList<CarBrandDto>>;

public class GetCarBrandsQueryHandler(IApplicationDbContext db) : IRequestHandler<GetCarBrandsQuery, IReadOnlyList<CarBrandDto>>
{
    public async Task<IReadOnlyList<CarBrandDto>> Handle(GetCarBrandsQuery request, CancellationToken ct) =>
        await db.CarBrands
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .Select(b => new CarBrandDto(
                b.Id,
                b.Name,
                b.Models.OrderBy(m => m.Name).Select(m => new CarModelDto(m.Id, m.Name, m.YearFrom, m.YearTo)).ToList()))
            .ToListAsync(ct);
}
