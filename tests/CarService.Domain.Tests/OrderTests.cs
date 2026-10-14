using CarService.Domain.Entities;
using CarService.Domain.Exceptions;

namespace CarService.Domain.Tests;

public class OrderTests
{
    private static Part Part(decimal price, int stock) =>
        new() { Sku = "SKU", Name = "Moy filtri", Manufacturer = "Mann", Price = price, StockQuantity = stock };

    [Fact]
    public void AddItem_recalculates_total()
    {
        var order = new Order();
        order.AddItem(Part(45_000, 10), 2, withInstallation: false);
        order.AddItem(Part(120_000, 5), 1, withInstallation: true);

        Assert.Equal(210_000, order.TotalAmount);
        Assert.Equal(2, order.Items.Count);
    }

    [Fact]
    public void AddItem_rejects_quantity_above_stock() =>
        Assert.Throws<DomainException>(() => new Order().AddItem(Part(10_000, 1), 2, false));

    [Fact]
    public void Booking_rejects_end_before_start()
    {
        var start = new DateTime(2026, 10, 20, 10, 0, 0, DateTimeKind.Utc);
        Assert.Throws<DomainException>(() => new Booking().Schedule(start, start.AddMinutes(-30)));
    }
}
