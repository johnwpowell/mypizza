using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MyPizza.Core.Data;
using MyPizza.Core.Data.Models;
using MyPizza.Core.Models;
using RabbitMQ.Client;
using Stripe;

namespace MyPizza.Core.Services
{
    public class PizzaOrderService
    {
        public async Task<int> OrderAsync(PizzaOrderModel pizzaOrderDto)
        {
            try
            {
                // validate
                if (!pizzaOrderDto.Pizzas.Any())
                {
                    throw new InvalidOperationException("An order must contain at least 1 pizza.");
                }

                if (pizzaOrderDto.Pizzas.Any(pizza => pizza.Toppings.Sum(x => x.Percentage) != 100))
                {
                    throw new InvalidOperationException("The entire pizza must be covered by toppings.");
                }

                // calculate total
                var total = 10 * pizzaOrderDto.Pizzas.Count;
                //todo: add sales tax
                //todo: enable coupons and specials

                // charge credit card
                var apiKey = "MyPizza API Key";
                var api = new StripeClient(apiKey);
                var card = Mapper.Map<CreditCard>(pizzaOrderDto.CreditCard);

                //dynamic response = api.CreateCharge(
                //    total,
                //    "usd", //todo: expand to other currencies
                //    card);

                //todo: credit card processing isn't working yet
                dynamic response = new
                {
                    IsError = false,
                    Paid = true
                };

                // check we got paid
                if (response.IsError || !response.Paid)
                {
                    throw new Exception("Payment failed. :(");
                }

                // save order in database
                var pizzaOrder = Mapper.Map<PizzaOrder>(pizzaOrderDto);
                pizzaOrder.Total = total;
                using (var context = new MyPizzaDbContext())
                {
                    context.PizzaOrders.Add(pizzaOrder);
                    await context.SaveChangesAsync();
                }

                // queue message for the store to make the pizza
                var factory = new ConnectionFactory { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var message = Encoding.UTF8.GetBytes(pizzaOrder.Id.ToString());
                        channel.BasicPublish("", "MyPizza", null, message);
                    }
                }

                // send email to user
                var client = new SmtpClient("localhost", 25);
                var from = new MailAddress("orders@mypizza.com", "My Pizza");
                var to = new MailAddress(pizzaOrderDto.EmailAddress);
                var mailMessage = new MailMessage(from, to)
                {
                    Subject = "MyPizza.com Order " + pizzaOrder.Id,
                    Body = "We are working on your cheesy awesome!"
                };

                //todo: get an smtp server
                client.Send(mailMessage);

                return pizzaOrder.Id;
            }
            catch (Exception ex)
            {
                // log exception
                var log = new EventLog {Source = "Application"};
                log.WriteEntry(ex.Message, EventLogEntryType.Error);

                throw;
            }
        }

        public async Task<int> OrderMenuItemAsync(int menuItem)
        {
            var order = new PizzaOrderModel
            {
                // all orders are free using our credit card
                CreditCard = new CreditCardModel
                {
                    Number = "5555137689",
                    Cvc = "123",
                    ExpMonth = 10,
                    ExpYear = 2020
                },
                EmailAddress = "papa.john@mypizza.com"
            };

            var pizza = new PizzaModel();
            order.Pizzas.Add(pizza);

            switch (menuItem)
            {
                case 1:
                    pizza.Toppings.Add(new PizzaToppingModel { Name = "Cheese", Percentage = 100 });
                    break;
                case 2:
                    pizza.Toppings.Add(new PizzaToppingModel { Name = "Pepperoni", Percentage = 100 });
                    break;
                default:
                    throw new Exception("Invalid menu item");
            }

            return await OrderAsync(order);
        }
    }
}