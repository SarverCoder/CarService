using CarService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

public class CarBrandConfiguration : IEntityTypeConfiguration<CarBrand>
{
    public void Configure(EntityTypeBuilder<CarBrand> b)
    {
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
    }
}

public class CarModelConfiguration : IEntityTypeConfiguration<CarModel>
{
    public void Configure(EntityTypeBuilder<CarModel> b)
    {
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.HasOne(x => x.Brand).WithMany(x => x.Models).HasForeignKey(x => x.BrandId);
        b.HasIndex(x => new { x.BrandId, x.Name }).IsUnique();
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.Property(x => x.NameUz).HasMaxLength(150).IsRequired();
        b.Property(x => x.NameRu).HasMaxLength(150).IsRequired();
        b.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PartConfiguration : IEntityTypeConfiguration<Part>
{
    public void Configure(EntityTypeBuilder<Part> b)
    {
        b.Property(x => x.Sku).HasMaxLength(64).IsRequired();
        b.HasIndex(x => x.Sku).IsUnique();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Manufacturer).HasMaxLength(100).IsRequired();
        b.Property(x => x.ImageUrl).HasMaxLength(500);
        b.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PartCompatibilityConfiguration : IEntityTypeConfiguration<PartCompatibility>
{
    public void Configure(EntityTypeBuilder<PartCompatibility> b)
    {
        b.HasKey(x => new { x.PartId, x.CarModelId });
        b.HasOne(x => x.Part).WithMany(x => x.Compatibilities).HasForeignKey(x => x.PartId);
        b.HasOne(x => x.CarModel).WithMany().HasForeignKey(x => x.CarModelId);
        b.HasIndex(x => x.CarModelId);
    }
}
