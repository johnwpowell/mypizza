using System.Diagnostics;

namespace MyPizza.Core.Services
{
    public interface ILoggingService
    {
        void LogError(string message);
    }

    public class LoggingService : ILoggingService
    {
        public void LogError(string message)
        {
            var log = new EventLog { Source = "Application" };
            log.WriteEntry(message, EventLogEntryType.Error);
        }
    }
}
