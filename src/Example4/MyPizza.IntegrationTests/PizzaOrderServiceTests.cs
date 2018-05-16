using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyPizza.Core;
using MyPizza.Core.Data;
using MyPizza.Core.Services;

namespace MyPizza.IntegrationTests
{
    [TestClass]
    public class PizzaOrderServiceTests
    {
        [TestInitialize]
        public void Setup()
        {
            AutoMapperConfig.Initialize();
        }

        [TestMethod]
        public async Task PizzaOrderServiceOrderTest()
        {
            var loggingService = new LoggingService();

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
                var orderId = await service.OrderMenuItemAsync(1);
                Assert.IsTrue(orderId != 0);
            }
        }
    }
}