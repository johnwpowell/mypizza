using System.Web;
using MyPizza.Core.Services;

namespace MyPizza.Web
{
    public class WebApiLoggingService : ILoggingService
    {
        public void LogError(string message)
        {
            if (HttpContext.Current.Request != null)
            {
                // todo log some http specific stuff
            }
        }
    }
}