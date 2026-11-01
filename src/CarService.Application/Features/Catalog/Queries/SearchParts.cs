using CarService.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarService.Application.Features.Catalog.Queries;

public record PartListItemDto(
    Guid Id,
    string Sku,
    string Name,
    string Manufacturer,
    bool IsOriginal,
    int WarrantyMonths,
    decimal Price,
    int StockQuantity,
    string? ImageUrl,
    Guid CategoryId);

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);

/// <summary>
/// Qismlarni marka yoki mashina modeli va yili, kategoriya va matn bo'yicha qidiradi.
/// Model berilsa, marka filtri e'tiborga olinmaydi.
/// Kategoriya tanlansa, uning bevosita bolalari ham kiradi.
/// </summary>
public record SearchPartsQuery(
    Guid? BrandId = null,
    Guid? CarModelId = null,
    int? Year = null,
    Guid? CategoryId = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<PartListItemDto>>;

public class SearchPartsQueryValidator : AbstractValidator<SearchPartsQuery>
{
    public SearchPartsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Year).InclusiveBetween(1950, 2100).When(x => x.Year.HasValue);
        RuleFor(x => x.Search).MaximumLength(100);
    }
}

public class SearchPartsQueryHandler(IApplicationDbContext db) : IRequestHandler<SearchPartsQuery, PagedResult<PartListItemDto>>
{
    public async Task<PagedResult<PartListItemDto>> Handle(SearchPartsQuery request, CancellationToken ct)
    {
        var query = db.Parts.AsNoTracking();

        if (request.CarModelId is { } modelId)
        {
            var year = request.Year;
            query = query.Where(p => p.Compatibilities.Any(c =>
                c.CarModelId == modelId &&
                (year == null || (c.YearFrom <= year && (c.YearTo == null || c.YearTo >= year)))));
        }
        else if (request.BrandId is { } brandId)
        {
            query = query.Where(p => p.Compatibilities.Any(c => c.CarModel.BrandId == brandId));
        }

        if (request.CategoryId is { } categoryId)
            query = query.Where(p => p.CategoryId == categoryId || p.Category.ParentId == categoryId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(term) || p.Sku.ToLower().Contains(term) || p.Manufacturer.ToLower().Contains(term));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.StockQuantity > 0)
            .ThenBy(p => p.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PartListItemDto(p.Id, p.Sku, p.Name, p.Manufacturer, p.IsOriginal, p.WarrantyMonths, p.Price, p.StockQuantity, p.ImageUrl, p.CategoryId))
            .ToListAsync(ct);

        return new PagedResult<PartListItemDto>(items, request.Page, request.PageSize, total);
    }
}
