namespace MyPizza.Core.Data.Models
{
    public enum OrderStatus
    {
        Pending,
        PaymentFailure,
        Store,
        Prep,
        Oven,
        OutForDelivery,
        Delivered
    }
}