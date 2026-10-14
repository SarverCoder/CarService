using CarService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarService.Application.Features.Services.Queries;

public record ServiceTypeDto(Guid Id, string NameUz, string NameRu, string? Description, int DurationMinutes, decimal PriceFrom, decimal PriceTo);

public record GetServicesQuery : IRequest<IReadOnlyList<ServiceTypeDto>>;

public class GetServicesQueryHandler(IApplicationDbContext db) : IRequestHandler<GetServicesQuery, IReadOnlyList<ServiceTypeDto>>
{
    public async Task<IReadOnlyList<ServiceTypeDto>> Handle(GetServicesQuery request, CancellationToken ct) =>
        await db.ServiceTypes
            .AsNoTracking()
            .OrderBy(s => s.NameUz)
            .Select(s => new ServiceTypeDto(s.Id, s.NameUz, s.NameRu, s.Description, s.DurationMinutes, s.PriceFrom, s.PriceTo))
            .ToListAsync(ct);
}
