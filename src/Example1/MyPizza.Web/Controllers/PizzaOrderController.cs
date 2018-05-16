using System;
using System.Threading.Tasks;
using System.Web.Http;
using MyPizza.Core.Services;

namespace MyPizza.Web.Controllers
{
    public class PizzaOrderController : ApiController
    {
        [HttpPost]
        [Route("api/pizzaorder/order/{menuItem:int}")]
        public async Task<IHttpActionResult> Order(int menuItem)
        {
            var service = new PizzaOrderService();

            try
            {
                var orderId = await service.OrderMenuItemAsync(menuItem);

                return Created("http://mypizza.com/orders/" + orderId, orderId);
            }
            catch (Exception ex)
            {
                //todo: log error with an exception filter

                return BadRequest("Oops, no pizza for you.");
            }
        }
    }
}