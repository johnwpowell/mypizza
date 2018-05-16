﻿using System;
using MyPizza.Core;
using MyPizza.Core.Services;

namespace MyPizza.PhoneApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoMapperConfig.Initialize();

            while (true)
            {
                Console.WriteLine("Press 1 for cheese, 2 for pepperoni. x to exit");
                var result = Console.ReadLine();

                if (result == "x")
                {
                    return;
                }

                try
                {
                    var service = new PizzaOrderService();
                    var task = service.OrderMenuItemAsync(int.Parse(result));
                    task.Wait();
                    var orderId = task.Result;
                    Console.WriteLine("Your order number is " + orderId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}