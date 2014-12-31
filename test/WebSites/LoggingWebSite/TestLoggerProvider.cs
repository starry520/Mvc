using System;
using Microsoft.Framework.Logging;

namespace LoggingWebSite
{
    public class TestLoggerProvider : ILoggerProvider
    {
        private readonly LogOptions _logOptions;
        private readonly TestSink _sink;

        public TestLoggerProvider(TestSink sink, LogOptions logOptions)
        {
            _sink = sink;
            _logOptions = logOptions;
        }

        public ILogger Create(string loggerName)
        {
            return new TestLogger(loggerName, _logOptions, _sink);
        }
    }
}