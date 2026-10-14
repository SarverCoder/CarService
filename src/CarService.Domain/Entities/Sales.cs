using CarService.Domain.Common;
using CarService.Domain.Enums;
using CarService.Domain.Exceptions;

namespace CarService.Domain.Entities;

public class Order : BaseEntity
{
    private readonly List<OrderItem> _items = [];

    public Guid UserId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.New;
    public DeliveryType DeliveryType { get; set; }
    public string? DeliveryAddress { get; set; }
    public decimal TotalAmount { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items;
    public ICollection<Payment> Payments { get; set; } = [];

    public void AddItem(Part part, int quantity, bool withInstallation)
    {
        if (quantity <= 0)
            throw new DomainException("Miqdor musbat bo'lishi kerak.");
        if (part.StockQuantity < quantity)
            throw new DomainException($"Omborda yetarli emas: {part.Name}");

        _items.Add(new OrderItem
        {
            OrderId = Id,
            PartId = part.Id,
            Quantity = quantity,
            UnitPrice = part.Price,
            WithInstallation = withInstallation
        });
        TotalAmount = _items.Sum(i => i.UnitPrice * i.Quantity);
    }
}

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid PartId { get; set; }
    public Part Part { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public bool WithInstallation { get; set; }
}

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? ExternalId { get; set; }
}
