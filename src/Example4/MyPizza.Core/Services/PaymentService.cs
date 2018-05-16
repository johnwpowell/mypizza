using AutoMapper;
using MyPizza.Core.Models;
using Stripe;

namespace MyPizza.Core.Services
{
    public interface IPaymentService
    {
        decimal? Process(PizzaOrderModel pizzaOrderDto);
    }

    public class PaymentService : IPaymentService
    {
        public decimal? Process(PizzaOrderModel pizzaOrderDto)
        {
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

            if (response.IsError || !response.Paid)
            {
                return null;
            }

            return total;
        }
    }
}
