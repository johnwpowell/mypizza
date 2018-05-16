using System;
using MyPizza.Core.Services;

namespace MyPizza.UnitTests.Fakes
{
    public class FakeLoggingService : ILoggingService
    {
        public void LogError(string message)
        {
            Console.WriteLine(message);
        }
    }
}