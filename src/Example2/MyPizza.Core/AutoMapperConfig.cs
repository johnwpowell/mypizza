using AutoMapper;
using MyPizza.Core.Data.Models;
using MyPizza.Core.Models;
using Stripe;

namespace MyPizza.Core
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<PizzaOrderModel, PizzaOrder>();
            CreateMap<PizzaModel, Pizza>();
            CreateMap<PizzaToppingModel, PizzaTopping>();
            CreateMap<CreditCardModel, CreditCard>();
        }

        public static void Initialize()
        {
            Mapper.Initialize(x => x.AddProfile<AutoMapperConfig>());
        }
    }
}