using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyPizza.Core;
using MyPizza.Core.Data;
using MyPizza.Core.Data.Models;
using MyPizza.Core.Models;
using MyPizza.Core.Services;
using MyPizza.UnitTests.Fakes;

namespace MyPizza.UnitTests
{
    [TestClass]
    public class PizzaOrderServiceTest
    {
        [TestMethod]
        public async Task PizzaOrderServiceOrderTest()
        {
            var loggingService = new Mock<ILoggingService>();
            var queueService = new Mock<IStoreQueueService>();
            var notificationService = new Mock<IOrderNotificationService>();

            var paymentService = new Mock<IPaymentService>();
            paymentService.Setup(x => x.Process(It.IsAny<PizzaOrderModel>())).Returns(42);

            var context = new Mock<MyPizzaDbContext>();
            var mockDbSet = TestHelper.MockDbSet(new List<PizzaOrder>());
            context.Setup(x => x.PizzaOrders).Returns(mockDbSet.Object);

            var service = new PizzaOrderService(
                context.Object,
                paymentService.Object,
                queueService.Object,
                notificationService.Object,
                loggingService.Object);

            await service.OrderMenuItemAsync(1);

            paymentService.Verify(x => x.Process(It.IsAny<PizzaOrderModel>()), Times.Once);
            queueService.Verify(x => x.Send(It.IsAny<PizzaOrderModel>()), Times.Once);
            notificationService.Verify(x => x.Send(It.IsAny<PizzaOrderModel>()), Times.Once);
            loggingService.Verify(x => x.LogError(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task PizzaOrderServiceOrderShouldFailIfPaymentFailsTest()
        {
            var loggingService = new Mock<ILoggingService>();
            var queueService = new Mock<IStoreQueueService>();
            var notificationService = new Mock<IOrderNotificationService>();

            var paymentService = new Mock<IPaymentService>();
            paymentService.Setup(x => x.Process(It.IsAny<PizzaOrderModel>())).Returns(default(decimal?));

            var context = new Mock<MyPizzaDbContext>();
            var mockDbSet = TestHelper.MockDbSet(new List<PizzaOrder>());
            context.Setup(x => x.PizzaOrders).Returns(mockDbSet.Object);

            var service = new PizzaOrderService(
                context.Object,
                paymentService.Object,
                queueService.Object,
                notificationService.Object,
                loggingService.Object);

            var expectedException = false;

            try
            {
                await service.OrderMenuItemAsync(1);
            }
            catch (Exception ex)
            {
                expectedException = ex.Message.Contains("Payment failed");
            }

            Assert.IsTrue(expectedException);
        }

        [TestMethod]
        public async Task PizzaOrderServiceOrderFakeTest()
        {
            // mock new Mock<ComplianceDbContext>();

            var loggingService = new FakeLoggingService();

            //db context requires a bit more effort to fake so let's abandon for now
            Assert.Inconclusive();

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

        [TestInitialize]
        public void TestInit()
        {
            AutoMapperConfig.Initialize();
        }
    }
}