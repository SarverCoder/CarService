using CarService.Domain.Common;

namespace CarService.Domain.Entities;

public class CarBrand : BaseEntity
{
    public string Name { get; set; } = null!;
    public ICollection<CarModel> Models { get; set; } = [];
}

public class CarModel : BaseEntity
{
    public Guid BrandId { get; set; }
    public CarBrand Brand { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int YearFrom { get; set; }
    public int? YearTo { get; set; }
}

public class Category : BaseEntity
{
    public string NameUz { get; set; } = null!;
    public string NameRu { get; set; } = null!;
    public Guid? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = [];
}

public class Part : BaseEntity
{
    public string Sku { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    public bool IsOriginal { get; set; }
    public int WarrantyMonths { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<PartCompatibility> Compatibilities { get; set; } = [];
}

/// <summary>Qaysi qism qaysi mashina modeli va yillariga mos kelishi.</summary>
public class PartCompatibility
{
    public Guid PartId { get; set; }
    public Part Part { get; set; } = null!;
    public Guid CarModelId { get; set; }
    public CarModel CarModel { get; set; } = null!;
    public int YearFrom { get; set; }
    public int? YearTo { get; set; }
}
