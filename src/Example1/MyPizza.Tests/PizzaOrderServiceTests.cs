using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyPizza.Core;
using MyPizza.Core.Services;

namespace MyPizza.Tests
{
    [TestClass]
    public class PizzaOrderServiceTests
    {
        [TestMethod]
        public async Task PizzaOrderServiceOrderTest()
        {
            // this test is brittle; often fails when a dependency has a transient fault
            // accidentally sent emails out one time
            // accidentally charged a credit card
            // this is an integration test
            // dependent on credit card service
            // dependent on database
            // dependent on SMTP server
            // dependent on RabbitMQ
            var service = new PizzaOrderService();
            var orderId = await service.OrderMenuItemAsync(1);
            Assert.IsTrue(orderId != 0);
        }

        [TestInitialize]
        public void Setup()
        {
            AutoMapperConfig.Initialize();
        }
    }
}