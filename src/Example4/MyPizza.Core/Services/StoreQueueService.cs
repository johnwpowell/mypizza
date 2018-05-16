using System.Text;
using MyPizza.Core.Models;
using RabbitMQ.Client;

namespace MyPizza.Core.Services
{
    public interface IStoreQueueService
    {
        void Send(PizzaOrderModel pizzaOrderDto);
    }

    public class StoreQueueService : IStoreQueueService
    {
        public void Send(PizzaOrderModel pizzaOrderDto)
        {
            // queue message for the store to make the pizza
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var message = Encoding.UTF8.GetBytes(pizzaOrderDto.Id.ToString());
                    channel.BasicPublish("", "MyPizza", null, message);
                }
            }
        }
    }
}
