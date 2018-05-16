using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyPizza.Core;
using MyPizza.Core.Services;

namespace MyPizza.Tests
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
            // this is an integration test
            // dependent on credit card service
            // dependent on database
            // dependent on SMTP server
            // dependent on RabbitMQ
            var service = new PizzaOrderService();
            var orderId = await service.OrderMenuItemAsync(1);
            Assert.IsTrue(orderId != 0);
        }
    }
}