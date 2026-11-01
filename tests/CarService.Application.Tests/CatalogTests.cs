using System.Text;
using CarService.Application.Common.Exceptions;
using CarService.Application.Features.Catalog.Commands;
using CarService.Application.Features.Catalog.Queries;
using CarService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarService.Application.Tests;

public class SearchPartsQueryTests
{
    private static async Task<(TestDbContext Db, CarModel Cobalt, Category Filters, Category Engine)> SeedAsync()
    {
        var db = TestDbContext.Create();
        var brand = new CarBrand { Name = "Chevrolet" };
        var cobalt = new CarModel { Brand = brand, Name = "Cobalt", YearFrom = 2013 };
        var lacetti = new CarModel { Brand = brand, Name = "Lacetti", YearFrom = 2004, YearTo = 2013 };
        var engine = new Category { NameUz = "Dvigatel", NameRu = "Двигатель" };
        var filters = new Category { NameUz = "Filtrlar", NameRu = "Фильтры", Parent = engine };

        var oilFilter = new Part { Sku = "A1", Name = "Moy filtri", Category = filters, Manufacturer = "GM", Price = 45_000, StockQuantity = 5 };
        oilFilter.Compatibilities.Add(new PartCompatibility { Part = oilFilter, CarModel = cobalt, YearFrom = 2013 });
        var oldPads = new Part { Sku = "B2", Name = "Kolodka", Category = engine, Manufacturer = "Bosch", Price = 200_000 };
        oldPads.Compatibilities.Add(new PartCompatibility { Part = oldPads, CarModel = lacetti, YearFrom = 2004, YearTo = 2013 });
        var cobaltOld = new Part { Sku = "C3", Name = "Eski filtr", Category = filters, Manufacturer = "GM", Price = 10_000 };
        cobaltOld.Compatibilities.Add(new PartCompatibility { Part = cobaltOld, CarModel = cobalt, YearFrom = 2013, YearTo = 2015 });

        db.AddRange(brand, cobalt, lacetti, engine, filters, oilFilter, oldPads, cobaltOld);
        await db.SaveChangesAsync();
        return (db, cobalt, filters, engine);
    }

    [Fact]
    public async Task Filters_by_model_and_year()
    {
        var (db, cobalt, _, _) = await SeedAsync();
        var handler = new SearchPartsQueryHandler(db);

        var all = await handler.Handle(new SearchPartsQuery(CarModelId: cobalt.Id), CancellationToken.None);
        var in2020 = await handler.Handle(new SearchPartsQuery(CarModelId: cobalt.Id, Year: 2020), CancellationToken.None);

        Assert.Equal(["A1", "C3"], all.Items.Select(p => p.Sku).Order());
        Assert.Equal(["A1"], in2020.Items.Select(p => p.Sku));
    }

    [Fact]
    public async Task Filters_by_brand_when_no_model_selected()
    {
        var (db, cobalt, _, _) = await SeedAsync();
        var other = new Part { Sku = "D4", Name = "Boshqa", Category = await db.Categories.FirstAsync(), Manufacturer = "X", Price = 1 };
        db.Parts.Add(other);
        await db.SaveChangesAsync();

        var result = await new SearchPartsQueryHandler(db).Handle(new SearchPartsQuery(BrandId: cobalt.BrandId), CancellationToken.None);

        Assert.Equal(3, result.TotalCount);
        Assert.DoesNotContain(result.Items, p => p.Sku == "D4");
    }

    [Fact]
    public async Task Parent_category_includes_child_categories()
    {
        var (db, _, _, engine) = await SeedAsync();

        var result = await new SearchPartsQueryHandler(db).Handle(new SearchPartsQuery(CategoryId: engine.Id), CancellationToken.None);

        Assert.Equal(3, result.TotalCount);
    }

    [Fact]
    public async Task Text_search_matches_name_sku_and_manufacturer_case_insensitively()
    {
        var (db, _, _, _) = await SeedAsync();
        var handler = new SearchPartsQueryHandler(db);

        Assert.Equal(["B2"], (await handler.Handle(new SearchPartsQuery(Search: "bosch"), CancellationToken.None)).Items.Select(p => p.Sku));
        Assert.Equal(["A1"], (await handler.Handle(new SearchPartsQuery(Search: "a1"), CancellationToken.None)).Items.Select(p => p.Sku));
    }

    [Fact]
    public async Task In_stock_parts_come_first()
    {
        var (db, _, _, _) = await SeedAsync();

        var result = await new SearchPartsQueryHandler(db).Handle(new SearchPartsQuery(), CancellationToken.None);

        Assert.Equal("A1", result.Items[0].Sku);
    }
}

public class GetPartByIdQueryTests
{
    [Fact]
    public async Task Throws_not_found_for_unknown_id()
    {
        await using var db = TestDbContext.Create();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            new GetPartByIdQueryHandler(db).Handle(new GetPartByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}

public class ImportPartsCommandTests
{
    private static Stream Csv(string text) => new MemoryStream(Encoding.UTF8.GetBytes(text));

    [Fact]
    public async Task Creates_parts_categories_and_models_then_updates_by_sku()
    {
        await using var db = TestDbContext.Create();
        var handler = new ImportPartsCommandHandler(db);

        var first = await handler.Handle(new ImportPartsCommand(Csv(
            "Sku;Name;Category;Manufacturer;IsOriginal;WarrantyMonths;Price;StockQuantity;ImageUrl;Compatibility\n" +
            "GM-1;Moy filtri;Filtrlar;GM;ha;6;45 000;10;;Chevrolet:Cobalt:2013-|Chevrolet:Gentra:2013-2020\n")), CancellationToken.None);

        Assert.Equal((1, 0), (first.Created, first.Updated));
        Assert.Empty(first.Errors);
        Assert.Equal(2, await db.CarModels.CountAsync());
        var part = await db.Parts.Include(p => p.Compatibilities).SingleAsync();
        Assert.True(part.IsOriginal);
        Assert.Equal(45_000, part.Price);
        Assert.Equal(2, part.Compatibilities.Count);

        var second = await handler.Handle(new ImportPartsCommand(Csv(
            "Sku,Name,Category,Manufacturer,Price,Compatibility\n" +
            "gm-1,\"Moy filtri, original\",Filtrlar,GM,50000,Chevrolet:Cobalt:2014-\n")), CancellationToken.None);

        Assert.Equal((0, 1), (second.Created, second.Updated));
        part = await db.Parts.Include(p => p.Compatibilities).SingleAsync();
        Assert.Equal("Moy filtri, original", part.Name);
        Assert.Equal(2014, part.Compatibilities.Single(c => c.CarModel.Name == "Cobalt").YearFrom);
        Assert.Equal(1, await db.Categories.CountAsync());
    }

    [Fact]
    public async Task Reports_bad_rows_and_missing_columns()
    {
        await using var db = TestDbContext.Create();
        var handler = new ImportPartsCommandHandler(db);

        var missing = await handler.Handle(new ImportPartsCommand(Csv("Sku;Name\nA;B\n")), CancellationToken.None);
        Assert.Contains("Category", missing.Errors.Single());

        var bad = await handler.Handle(new ImportPartsCommand(Csv(
            "Sku;Name;Category;Manufacturer;Price;Compatibility\n" +
            "A;Filtr;Filtrlar;GM;abc;\n" +
            "B;Filtr;Filtrlar;GM;100;Chevrolet-Cobalt\n" +
            "C;Filtr;Filtrlar;GM;100;\n")), CancellationToken.None);

        Assert.Equal(1, bad.Created);
        Assert.Equal(2, bad.Errors.Count);
        Assert.StartsWith("2-qator", bad.Errors[0]);
        Assert.StartsWith("3-qator", bad.Errors[1]);
    }
}
