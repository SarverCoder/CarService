namespace CarService.Domain.Enums;

public enum BookingStatus { Pending, Confirmed, InProgress, Completed, Cancelled }

public enum OrderStatus { New, Paid, ReadyForPickup, Shipped, Completed, Cancelled }

public enum DeliveryType { Pickup, Delivery }

public enum PaymentMethod { Cash, Click, Payme, Uzum }

public enum PaymentStatus { Pending, Succeeded, Failed, Refunded }

public enum AiChannel { Web, Telegram }

public enum AiMessageRole { User, Assistant }
