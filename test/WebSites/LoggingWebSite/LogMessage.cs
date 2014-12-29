using System;
using Microsoft.AspNet.Diagnostics.Elm;
using Microsoft.Framework.Logging;

namespace LoggingWebSite
{
    public class LogMessage
    {
        public int EventID { get; set; }

        public string Message { get; set; }

        public string Name { get; set; }

        public LogLevel Severity { get; set; }

        public object State { get; set; }

        public DateTimeOffset Time { get; set; }

        public HttpInfo HttpInfo { get; set; }
    }
}