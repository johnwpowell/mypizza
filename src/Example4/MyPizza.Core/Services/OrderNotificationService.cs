using System.Net.Mail;
using MyPizza.Core.Models;

namespace MyPizza.Core.Services
{
    public interface IOrderNotificationService
    {
        void Send(PizzaOrderModel pizzaOrderDto);
    }

    public class OrderNotificationService : IOrderNotificationService
    {
        public void Send(PizzaOrderModel pizzaOrderDto)
        {
            var client = new SmtpClient("localhost", 25);
            var from = new MailAddress("orders@mypizza.com", "My Pizza");
            var to = new MailAddress(pizzaOrderDto.EmailAddress);
            var mailMessage = new MailMessage(from, to)
            {
                Subject = "MyPizza.com Order " + pizzaOrderDto.Id,
                Body = "We are working on your cheesy awesome!"
            };

            //todo: get an smtp server
            client.Send(mailMessage);
        }
    }
}
