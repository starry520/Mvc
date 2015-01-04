using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;

namespace LoggingWebSite
{
    public static class LogExtensions
    {
        public static IApplicationBuilder UseLogCapture(this IApplicationBuilder builder)
        {
            // add the log provider to any registered log factory here so the logger can start capturing logs immediately
            var factory = builder.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var sink = builder.ApplicationServices.GetRequiredService<TestSink>();
            var options = builder.ApplicationServices.GetService<IOptions<LogOptions>>();
            factory.AddProvider(new TestLoggerProvider(sink, options?.Options ?? new LogOptions()));

            return builder.UseMiddleware<LogCaptureMiddleware>();
        }
    }
}