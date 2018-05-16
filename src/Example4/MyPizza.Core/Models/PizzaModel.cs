using System.Collections.Generic;

namespace MyPizza.Core.Models
{
    public class PizzaModel
    {
        public ICollection<PizzaToppingModel> Toppings { get; set; } = new HashSet<PizzaToppingModel>();
    }
}
