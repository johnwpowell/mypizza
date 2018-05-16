using System;
using System.Threading.Tasks;
using System.Web.Http;
using MyPizza.Core.Services;

namespace MyPizza.Web.Controllers
{
    public class PizzaOrderController : ApiController
    {
        private readonly IPizzaOrderService pizzaOrderService;
        private readonly ILoggingService loggingService;

        public PizzaOrderController(
            IPizzaOrderService pizzaOrderService,
            ILoggingService loggingService)
        {
            this.pizzaOrderService = pizzaOrderService;
            this.loggingService = loggingService;
        }

        [HttpPost]
        [Route("api/pizzaorder/order/{menuItem:int}")]
        public async Task<IHttpActionResult> Order(int menuItem)
        {
            try
            {
                // so much cleaner and DRY
                var orderId = await pizzaOrderService.OrderMenuItemAsync(menuItem);

                return Created("http://mypizza.com/orders/" + orderId, orderId);
            }
            catch (Exception ex)
            {
                loggingService.LogError(ex.Message);

                return BadRequest("Oops, no pizza for you.");
            }
        }
    }
}