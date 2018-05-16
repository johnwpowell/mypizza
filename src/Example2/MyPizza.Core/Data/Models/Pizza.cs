using System.Collections.Generic;

namespace MyPizza.Core.Data.Models
{
    public class Pizza
    {
        public int Id { get; set; }

        public virtual ICollection<PizzaTopping> Toppings { get; set; } = new HashSet<PizzaTopping>();
    }
}