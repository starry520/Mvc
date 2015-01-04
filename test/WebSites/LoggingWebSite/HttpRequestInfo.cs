using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LoggingWebSite
{
    public class HttpRequestInfo
    {
        /// <summary>
        /// This value is set by the <see cref="LogCaptureMiddleware"/> when a 
        /// request is received.
        /// </summary>
        public Guid RequestID { get; set; }

        public string Host { get; set; }

        public string Path { get; set; }
        
        public string Scheme { get; set; }

        public string Method { get; set; }

        public string Protocol { get; set; }

        public string ContentType { get; set; }

        public IEnumerable<KeyValuePair<string, string[]>> Headers { get; set; }

        public IEnumerable<KeyValuePair<string, string[]>> Query { get; set; }

        public IEnumerable<KeyValuePair<string, string[]>> Cookies { get; set; }
    }
}