using CarService.Application.Common.Exceptions;
using CarService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarService.Application.Features.Catalog.Queries;

public record PartCompatibilityDto(Guid CarModelId, string BrandName, string ModelName, int YearFrom, int? YearTo);

public record PartDetailsDto(
    Guid Id,
    string Sku,
    string Name,
    string Manufacturer,
    bool IsOriginal,
    int WarrantyMonths,
    decimal Price,
    int StockQuantity,
    string? ImageUrl,
    CategoryDto Category,
    IReadOnlyList<PartCompatibilityDto> Compatibilities);

public record GetPartByIdQuery(Guid Id) : IRequest<PartDetailsDto>;

public class GetPartByIdQueryHandler(IApplicationDbContext db) : IRequestHandler<GetPartByIdQuery, PartDetailsDto>
{
    public async Task<PartDetailsDto> Handle(GetPartByIdQuery request, CancellationToken ct) =>
        await db.Parts
            .AsNoTracking()
            .Where(p => p.Id == request.Id)
            .Select(p => new PartDetailsDto(
                p.Id, p.Sku, p.Name, p.Manufacturer, p.IsOriginal, p.WarrantyMonths, p.Price, p.StockQuantity, p.ImageUrl,
                new CategoryDto(p.Category.Id, p.Category.NameUz, p.Category.NameRu, p.Category.ParentId),
                p.Compatibilities
                    .OrderBy(c => c.CarModel.Brand.Name).ThenBy(c => c.CarModel.Name)
                    .Select(c => new PartCompatibilityDto(c.CarModelId, c.CarModel.Brand.Name, c.CarModel.Name, c.YearFrom, c.YearTo))
                    .ToList()))
            .FirstOrDefaultAsync(ct)
        ?? throw new NotFoundException("Qism topilmadi.");
}
