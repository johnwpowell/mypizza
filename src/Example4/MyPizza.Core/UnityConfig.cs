using MyPizza.Core.Data;
using MyPizza.Core.Services;
using Unity;

namespace MyPizza.Core
{
    public class UnityConfig
    {
        public static UnityContainer GetDefaultContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<MyPizzaDbContext, MyPizzaDbContext>();
            container.RegisterType<ILoggingService, LoggingService>();
            container.RegisterType<IOrderNotificationService, OrderNotificationService>();
            container.RegisterType<IPaymentService, PaymentService>();
            container.RegisterType<IPizzaOrderService, PizzaOrderService>();
            container.RegisterType<IStoreQueueService, StoreQueueService>();

            return container;
        }
    }
}
