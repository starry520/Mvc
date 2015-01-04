using System;
using Microsoft.Framework.Logging;

namespace LoggingWebSite
{
    public class TestLogger : ILogger
    {
        private readonly string _loggerName;
        private readonly LogOptions _logOptions;
        private readonly TestSink _sink;

        public TestLogger(string loggerName, LogOptions logOptions, TestSink sink)
        {
            _loggerName = loggerName;
            _logOptions = logOptions;
            _sink = sink;
        }

        public IDisposable BeginScope(object state)
        {
            return LogScope.Push(new LogScope(state), _sink);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logOptions.Filter(_loggerName, logLevel);
        }

        public void Write(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel) || (state == null && exception == null))
            {
                return;
            }

            var messageNode = new MessageNode()
            {
                LoggerName = _loggerName,
                EventID = eventId,
                State = state,
                StateType = state?.GetType(),
                Exception = exception,
                LogLevel = logLevel,
#if ASPNET50 || ASPNETCORE50
                RequestInfo = RequestContext.Current?.RequestInfo
#endif
            };

            if (LogScope.CurrentScope != null)
            {
                // add the message to the ScopeNode present in the sink
                LogScope.CurrentScope.ScopeNode.Children.Add(messageNode);
            }
            else
            {
                // The log does not belong to any scope
                _sink.LogEntries.Add(messageNode);
            }
        }
    }
}