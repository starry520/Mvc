using System;
using Microsoft.Framework.Logging;

namespace LoggingWebSite
{
    public class MessageNode : LogNode
    {
        public string LoggerName { get; set; }

        public LogLevel LogLevel { get; set; }

        public int EventID { get; set; }

        public Exception Exception { get; set; }
    }
}