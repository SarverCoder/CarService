using System.Globalization;
using System.Text;
using CarService.Application.Interfaces;
using CarService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CarService.Application.Features.Catalog.Commands;

public record ImportPartsResult(int Created, int Updated, IReadOnlyList<string> Errors);

/// <summary>
/// CSV'dan qismlarni import qiladi (Excel'da "CSV" sifatida saqlangan fayl). SKU bo'yicha mavjud qism yangilanadi.
/// Ustunlar: Sku, Name, Category, Manufacturer, IsOriginal, WarrantyMonths, Price, StockQuantity, ImageUrl, Compatibility.
/// Compatibility: "Marka:Model:2013-2023" ko'rinishida, bir nechtasi "|" bilan ajratiladi; yil oxiri bo'sh bo'lsa hozirgacha.
/// Topilmagan kategoriya, marka va modellar avtomatik yaratiladi.
/// </summary>
public record ImportPartsCommand(Stream Content) : IRequest<ImportPartsResult>;

public class ImportPartsCommandHandler(IApplicationDbContext db) : IRequestHandler<ImportPartsCommand, ImportPartsResult>
{
    private static readonly string[] RequiredColumns = ["Sku", "Name", "Category", "Manufacturer", "Price"];

    public async Task<ImportPartsResult> Handle(ImportPartsCommand request, CancellationToken ct)
    {
        using var reader = new StreamReader(request.Content, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        var headerLine = await reader.ReadLineAsync(ct);
        if (string.IsNullOrWhiteSpace(headerLine))
            return new ImportPartsResult(0, 0, ["Fayl bo'sh."]);

        // Excel o'zbek/rus lokalida ";" bilan saqlaydi.
        var delimiter = headerLine.Contains(';') ? ';' : ',';
        var header = SplitLine(headerLine, delimiter).Select(h => h.Trim()).ToList();
        var missing = RequiredColumns.Where(c => !header.Contains(c, StringComparer.OrdinalIgnoreCase)).ToList();
        if (missing.Count > 0)
            return new ImportPartsResult(0, 0, [$"Ustunlar yetishmaydi: {string.Join(", ", missing)}"]);

        var columns = header.Select((name, i) => (name, i)).ToDictionary(x => x.name, x => x.i, StringComparer.OrdinalIgnoreCase);

        var categories = await db.Categories.ToListAsync(ct);
        var brands = await db.CarBrands.Include(b => b.Models).ToListAsync(ct);
        var parts = await db.Parts.Include(p => p.Compatibilities).ToDictionaryAsync(p => p.Sku, StringComparer.OrdinalIgnoreCase, ct);

        int created = 0, updated = 0, lineNumber = 1;
        var errors = new List<string>();

        while (await reader.ReadLineAsync(ct) is { } line)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var cells = SplitLine(line, delimiter);
            string Cell(string name) =>
                columns.TryGetValue(name, out var i) && i < cells.Count ? cells[i].Trim() : string.Empty;

            var sku = Cell("Sku");
            var name = Cell("Name");
            var categoryName = Cell("Category");
            if (sku.Length == 0 || name.Length == 0 || categoryName.Length == 0)
            {
                errors.Add($"{lineNumber}-qator: Sku, Name va Category to'ldirilishi shart.");
                continue;
            }
            if (!TryParseDecimal(Cell("Price"), out var price) || price < 0)
            {
                errors.Add($"{lineNumber}-qator: narx noto'g'ri ({Cell("Price")}).");
                continue;
            }

            List<(CarModel Model, int From, int? To)> compatibilities;
            try
            {
                compatibilities = ParseCompatibility(Cell("Compatibility"), brands);
            }
            catch (FormatException ex)
            {
                errors.Add($"{lineNumber}-qator: {ex.Message}");
                continue;
            }

            var category = categories.FirstOrDefault(c => string.Equals(c.NameUz, categoryName, StringComparison.OrdinalIgnoreCase));
            if (category is null)
            {
                category = new Category { NameUz = categoryName, NameRu = categoryName };
                categories.Add(category);
                db.Categories.Add(category);
            }

            if (!parts.TryGetValue(sku, out var part))
            {
                part = new Part { Sku = sku };
                parts[sku] = part;
                db.Parts.Add(part);
                created++;
            }
            else
            {
                updated++;
            }

            part.Name = name;
            part.Category = category;
            part.Manufacturer = Cell("Manufacturer");
            part.IsOriginal = ParseBool(Cell("IsOriginal"));
            part.WarrantyMonths = int.TryParse(Cell("WarrantyMonths"), out var warranty) ? warranty : 0;
            part.Price = price;
            part.StockQuantity = int.TryParse(Cell("StockQuantity"), out var stock) ? stock : 0;
            part.ImageUrl = Cell("ImageUrl") is { Length: > 0 } url ? url : null;

            foreach (var (model, from, to) in compatibilities)
            {
                var existing = part.Compatibilities.FirstOrDefault(c => c.CarModelId == model.Id || c.CarModel == model);
                if (existing is null)
                    part.Compatibilities.Add(new PartCompatibility { Part = part, CarModel = model, YearFrom = from, YearTo = to });
                else
                    (existing.YearFrom, existing.YearTo) = (from, to);
            }
        }

        await db.SaveChangesAsync(ct);
        return new ImportPartsResult(created, updated, errors);
    }

    private List<(CarModel, int, int?)> ParseCompatibility(string raw, List<CarBrand> brands)
    {
        var result = new List<(CarModel, int, int?)>();
        foreach (var entry in raw.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = entry.Split(':', StringSplitOptions.TrimEntries);
            if (parts.Length != 3 || parts[0].Length == 0 || parts[1].Length == 0)
                throw new FormatException($"moslik noto'g'ri: \"{entry}\" (kutilgan: Marka:Model:2013-2023).");

            var years = parts[2].Split('-', StringSplitOptions.TrimEntries);
            if (!int.TryParse(years[0], out var from))
                throw new FormatException($"yil noto'g'ri: \"{entry}\".");
            int? to = years.Length > 1 && years[1].Length > 0
                ? int.TryParse(years[1], out var y) ? y : throw new FormatException($"yil noto'g'ri: \"{entry}\".")
                : null;

            var brand = brands.FirstOrDefault(b => string.Equals(b.Name, parts[0], StringComparison.OrdinalIgnoreCase));
            if (brand is null)
            {
                brand = new CarBrand { Name = parts[0] };
                brands.Add(brand);
                db.CarBrands.Add(brand);
            }

            var model = brand.Models.FirstOrDefault(m => string.Equals(m.Name, parts[1], StringComparison.OrdinalIgnoreCase));
            if (model is null)
            {
                model = new CarModel { Brand = brand, Name = parts[1], YearFrom = from, YearTo = to };
                brand.Models.Add(model);
                db.CarModels.Add(model);
            }

            result.Add((model, from, to));
        }
        return result;
    }

    private static bool TryParseDecimal(string value, out decimal result) =>
        decimal.TryParse(value.Replace(" ", string.Empty).Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out result);

    private static bool ParseBool(string value) =>
        value.Trim().ToLowerInvariant() is "1" or "true" or "ha" or "да" or "yes";

    /// <summary>Qo'shtirnoqli maydonlarni hisobga olgan holda CSV qatorini bo'ladi.</summary>
    private static List<string> SplitLine(string line, char delimiter)
    {
        var cells = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;
        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (inQuotes)
            {
                if (ch == '"' && i + 1 < line.Length && line[i + 1] == '"') { current.Append('"'); i++; }
                else if (ch == '"') inQuotes = false;
                else current.Append(ch);
            }
            else if (ch == '"') inQuotes = true;
            else if (ch == delimiter) { cells.Add(current.ToString()); current.Clear(); }
            else current.Append(ch);
        }
        cells.Add(current.ToString());
        return cells;
    }
}
