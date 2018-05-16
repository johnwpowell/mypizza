using System.Data.Entity;
using MyPizza.Core.Data.Models;

namespace MyPizza.Core.Data
{
    public class MyPizzaDbContext : DbContext
    {
        public MyPizzaDbContext() : base("MyPizzaContext")
        {
        }

        public virtual DbSet<PizzaOrder> PizzaOrders { get; set; }
    }
}