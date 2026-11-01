using CarService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarService.Application.Features.Catalog.Queries;

public record CategoryDto(Guid Id, string NameUz, string NameRu, Guid? ParentId);

public record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;

public class GetCategoriesQueryHandler(IApplicationDbContext db) : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct) =>
        await db.Categories
            .AsNoTracking()
            .OrderBy(c => c.NameUz)
            .Select(c => new CategoryDto(c.Id, c.NameUz, c.NameRu, c.ParentId))
            .ToListAsync(ct);
}
