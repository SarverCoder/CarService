using CarService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarService.Infrastructure.Persistence.Seed;

/// <summary>Bo'sh bazaga namunaviy katalog: O'zbekistonda ko'p uchraydigan modellar, kategoriyalar va qismlar.</summary>
internal static class CatalogSeed
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.CarBrands.AnyAsync()) return;

        var chevrolet = new CarBrand { Name = "Chevrolet" };
        var cobalt = Model(chevrolet, "Cobalt", 2013);
        var nexia3 = Model(chevrolet, "Nexia 3", 2016, 2020);
        var gentra = Model(chevrolet, "Gentra", 2013);
        var lacetti = Model(chevrolet, "Lacetti", 2004, 2013);
        var spark = Model(chevrolet, "Spark", 2010, 2022);
        var damas = Model(chevrolet, "Damas", 1996);
        var malibu = Model(chevrolet, "Malibu", 2012);

        var kia = new CarBrand { Name = "Kia" };
        var k5 = Model(kia, "K5", 2020);

        var hyundai = new CarBrand { Name = "Hyundai" };
        var elantra = Model(hyundai, "Elantra", 2016);

        db.CarBrands.AddRange(chevrolet, kia, hyundai);

        var engine = new Category { NameUz = "Dvigatel", NameRu = "Двигатель" };
        var filters = new Category { NameUz = "Filtrlar", NameRu = "Фильтры", Parent = engine };
        var oils = new Category { NameUz = "Moylar", NameRu = "Масла", Parent = engine };
        var brakes = new Category { NameUz = "Tormoz tizimi", NameRu = "Тормозная система" };
        var pads = new Category { NameUz = "Tormoz kolodkalari", NameRu = "Тормозные колодки", Parent = brakes };
        var discs = new Category { NameUz = "Tormoz disklari", NameRu = "Тормозные диски", Parent = brakes };
        var electrics = new Category { NameUz = "Elektr jihozlari", NameRu = "Электрооборудование" };
        var plugs = new Category { NameUz = "Svechalar", NameRu = "Свечи зажигания", Parent = electrics };
        var suspension = new Category { NameUz = "Osma (xodovoy)", NameRu = "Подвеска" };
        db.Categories.AddRange(engine, filters, oils, brakes, pads, discs, electrics, plugs, suspension);

        db.Parts.AddRange(
            Part("GM-25183779", "Moy filtri", filters, "GM", true, 6, 45_000, 40, (cobalt, 2013, null), (gentra, 2013, null), (nexia3, 2016, 2020), (lacetti, 2004, 2013)),
            Part("MANN-W712/95", "Moy filtri MANN W 712/95", filters, "MANN-FILTER", false, 6, 38_000, 25, (cobalt, 2013, null), (gentra, 2013, null), (lacetti, 2004, 2013)),
            Part("GM-96950990", "Havo filtri", filters, "GM", true, 6, 65_000, 18, (cobalt, 2013, null), (nexia3, 2016, 2020)),
            Part("GM-96827723", "Salon filtri", filters, "GM", true, 6, 55_000, 0, (gentra, 2013, null), (lacetti, 2004, 2013)),
            Part("GM-93743595", "Dvigatel moyi 5W-30 Dexos2, 4 L", oils, "GM", true, 0, 420_000, 30, (cobalt, 2013, null), (gentra, 2013, null), (nexia3, 2016, 2020), (malibu, 2012, null)),
            Part("BOSCH-0986494524", "Old tormoz kolodkalari", pads, "Bosch", false, 12, 210_000, 12, (cobalt, 2013, null), (nexia3, 2016, 2020)),
            Part("GM-96534653", "Old tormoz kolodkalari", pads, "GM", true, 12, 280_000, 8, (gentra, 2013, null), (lacetti, 2004, 2013)),
            Part("GM-13301234", "Old tormoz kolodkalari", pads, "GM", true, 12, 650_000, 4, (malibu, 2012, null)),
            Part("TRW-DF4823", "Old tormoz diski", discs, "TRW", false, 12, 390_000, 6, (cobalt, 2013, null), (gentra, 2013, null)),
            Part("NGK-BKR6E", "Svecha NGK BKR6E", plugs, "NGK", false, 6, 35_000, 60, (cobalt, 2013, null), (nexia3, 2016, 2020), (spark, 2010, 2022), (damas, 1996, null)),
            Part("GM-96130723", "Old amortizator", suspension, "GM", true, 12, 520_000, 5, (damas, 1996, null)),
            Part("KIA-26300-35505", "Moy filtri", filters, "Hyundai Mobis", true, 6, 75_000, 20, (k5, 2020, null), (elantra, 2016, null)),
            Part("KIA-58101-L3A00", "Old tormoz kolodkalari", pads, "Hyundai Mobis", true, 12, 720_000, 3, (k5, 2020, null)));

        await db.SaveChangesAsync();
    }

    private static CarModel Model(CarBrand brand, string name, int yearFrom, int? yearTo = null)
    {
        var model = new CarModel { Brand = brand, Name = name, YearFrom = yearFrom, YearTo = yearTo };
        brand.Models.Add(model);
        return model;
    }

    private static Part Part(string sku, string name, Category category, string manufacturer, bool isOriginal, int warrantyMonths,
        decimal price, int stock, params (CarModel Model, int From, int? To)[] fits)
    {
        var part = new Part
        {
            Sku = sku, Name = name, Category = category, Manufacturer = manufacturer, IsOriginal = isOriginal,
            WarrantyMonths = warrantyMonths, Price = price, StockQuantity = stock
        };
        foreach (var (model, from, to) in fits)
            part.Compatibilities.Add(new PartCompatibility { Part = part, CarModel = model, YearFrom = from, YearTo = to });
        return part;
    }
}
