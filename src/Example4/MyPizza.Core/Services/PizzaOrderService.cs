using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyPizza.Core.Data;
using MyPizza.Core.Data.Models;
using MyPizza.Core.Models;

namespace MyPizza.Core.Services
{
    public interface IPizzaOrderService
    {
        Task<int> OrderAsync(PizzaOrderModel pizzaOrderDto);

        Task<int> OrderMenuItemAsync(int menuItem);
    }

    public class PizzaOrderService : IPizzaOrderService
    {
        private readonly MyPizzaDbContext context;
        private readonly IPaymentService paymentService;
        private readonly IStoreQueueService storeQueueService;
        private readonly IOrderNotificationService orderNotificationService;
        private readonly ILoggingService loggingService;

        public PizzaOrderService(
            MyPizzaDbContext context,
            IPaymentService paymentService,
            IStoreQueueService storeQueueService,
            IOrderNotificationService orderNotificationService,
            ILoggingService loggingService)
        {
            this.context = context;
            this.paymentService = paymentService;
            this.storeQueueService = storeQueueService;
            this.orderNotificationService = orderNotificationService;
            this.loggingService = loggingService;
        }

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

                // process the payment
                var total = paymentService.Process(pizzaOrderDto);

                // check we got paid
                if (!total.HasValue)
                {
                    throw new Exception("Payment failed. :(");
                }

                // save order in database
                var pizzaOrder = Mapper.Map<PizzaOrder>(pizzaOrderDto);
                pizzaOrder.Total = total.Value;
                context.PizzaOrders.Add(pizzaOrder);

                await context.SaveChangesAsync();

                pizzaOrderDto.Id = pizzaOrder.Id;

                // queue message for the store to make the pizza
                storeQueueService.Send(pizzaOrderDto);

                // send email to user
                orderNotificationService.Send(pizzaOrderDto);

                return pizzaOrderDto.Id;
            }
            catch (Exception ex)
            {
                // log exception
                loggingService.LogError(ex.Message);

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