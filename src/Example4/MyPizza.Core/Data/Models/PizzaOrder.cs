using System.Collections.Generic;

namespace MyPizza.Core.Data.Models
{
    public class PizzaOrder
    {
        public int Id { get; set; }

        public OrderStatus Status { get; set; }

        public decimal Total { get; set; }

        public virtual ICollection<Pizza> Pizzas { get; set; } = new HashSet<Pizza>();
    }
}