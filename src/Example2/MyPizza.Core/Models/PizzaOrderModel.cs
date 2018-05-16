using System.Collections.Generic;

namespace MyPizza.Core.Models
{
    public class PizzaOrderModel
    {
        public int Id { get; set; }

        public string EmailAddress { get; set; }

        public ICollection<PizzaModel> Pizzas { get; set; } = new HashSet<PizzaModel>();

        public CreditCardModel CreditCard { get; set; }
    }
}