using System;
using System.Collections.Generic;
using Microsoft.AspNet.Diagnostics.Elm;
using Microsoft.Framework.Logging;

namespace LoggingWebSite
{
    public class LogMessage
    {
        public int EventID { get; set; }

        public string Message { get; set; }

        public string LoggerName { get; set; }

        public LogLevel Severity { get; set; }

        public object State { get; set; }

        public DateTimeOffset Time { get; set; }

        public HttpRequestInfo RequestInfo { get; set; }
    }

    public class HttpRequestInfo
    {
        public Guid RequestID { get; set; }

        public string Host { get; set; }

        public string Path { get; set; }

        public string ContentType { get; set; }

        public string Scheme { get; set; }

        public int StatusCode { get; set; }

        public string Method { get; set; }

        public string Protocol { get; set; }

        public IEnumerable<KeyValuePair<string, string[]>> Headers { get; set; }

        public string Query { get; set; }

        public IEnumerable<KeyValuePair<string, string[]>> Cookies { get; set; }
    }
}