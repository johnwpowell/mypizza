using System;
using MyPizza.Core.Services;

namespace MyPizza.PhoneApp
{
    public class ConsoleLoggingService : ILoggingService
    {
        public void LogError(string message)
        {
            Console.WriteLine(message);
        }
    }
}
