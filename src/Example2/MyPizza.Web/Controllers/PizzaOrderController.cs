using System;
using System.Threading.Tasks;
using System.Web.Http;
using MyPizza.Core.Data;
using MyPizza.Core.Services;

namespace MyPizza.Web.Controllers
{
    public class PizzaOrderController : ApiController
    {
        [HttpPost]
        [Route("api/pizzaorder/order/{menuItem:int}")]
        public async Task<IHttpActionResult> Order(int menuItem)
        {
            // this is painful,
            //  but we can swap out concrete implementation
            //  unfortunately, this is not DRY
            var loggingService = new WebApiLoggingService();

            using (var context = new MyPizzaDbContext())
            {
                var queueService = new StoreQueueService();
                var paymentService = new PaymentService();
                var notificationService = new OrderNotificationService();

                var service = new PizzaOrderService(
                    context, 
                    paymentService, 
                    queueService,
                    notificationService,
                    loggingService);

                try
                {
                    var orderId = await service.OrderMenuItemAsync(menuItem);

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
}