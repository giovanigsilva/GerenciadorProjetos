using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ProjectManagement.Api.Extensions
{
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddCustomLogging(this ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();

            return logging;
        }
    }
}
