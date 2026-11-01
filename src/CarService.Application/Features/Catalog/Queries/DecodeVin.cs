using CarService.Application.Interfaces;
using CarService.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarService.Application.Features.Catalog.Queries;

/// <summary>VIN'dan aniqlangan marka va yil. Modelni VIN'dan ishonchli aniqlab bo'lmaydi, uni mijoz tanlaydi.</summary>
public record VinDecodeResult(string Vin, string Wmi, Guid? BrandId, string? BrandName, int? Year);

public record DecodeVinQuery(string Vin) : IRequest<VinDecodeResult>;

public class DecodeVinQueryHandler(IApplicationDbContext db, TimeProvider clock) : IRequestHandler<DecodeVinQuery, VinDecodeResult>
{
    // O'zbekistonda ko'p uchraydigan ishlab chiqaruvchilar (WMI → marka nomi).
    private static readonly Dictionary<string, string> WmiBrands = new(StringComparer.OrdinalIgnoreCase)
    {
        ["XWB"] = "Chevrolet", ["XWF"] = "Chevrolet", ["KL1"] = "Chevrolet", ["KLA"] = "Chevrolet", ["1G1"] = "Chevrolet",
        ["KMH"] = "Hyundai", ["KMJ"] = "Hyundai", ["5NP"] = "Hyundai",
        ["KNA"] = "Kia", ["KND"] = "Kia", ["KNE"] = "Kia",
        ["JTD"] = "Toyota", ["JTE"] = "Toyota", ["JTM"] = "Toyota", ["JTN"] = "Toyota", ["4T1"] = "Toyota",
        ["WVW"] = "Volkswagen", ["WV1"] = "Volkswagen", ["WV2"] = "Volkswagen",
        ["XTA"] = "Lada", ["WBA"] = "BMW", ["WDD"] = "Mercedes-Benz", ["WDB"] = "Mercedes-Benz",
        ["LGX"] = "BYD", ["LC0"] = "BYD", ["LVV"] = "Chery", ["LB3"] = "Geely"
    };

    public async Task<VinDecodeResult> Handle(DecodeVinQuery request, CancellationToken ct)
    {
        var vin = VinCode.Create(request.Vin);
        var year = vin.GetModelYear(clock.GetUtcNow().Year);

        if (!WmiBrands.TryGetValue(vin.Wmi, out var brandName))
            return new VinDecodeResult(vin.Value, vin.Wmi, null, null, year);

        var brand = await db.CarBrands.AsNoTracking()
            .Where(b => b.Name.ToLower() == brandName.ToLower())
            .Select(b => new { b.Id, b.Name })
            .FirstOrDefaultAsync(ct);

        return new VinDecodeResult(vin.Value, vin.Wmi, brand?.Id, brand?.Name ?? brandName, year);
    }
}
