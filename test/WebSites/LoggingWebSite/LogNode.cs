using System;

namespace LoggingWebSite
{
    public class LogNode
    {
        public HttpRequestInfo RequestInfo { get; set; }

        public object State { get; set; }

        /// <summary>
        /// Type of the property <seealso cref="State"/> for clients
        /// to easily consume it.
        /// </summary>
        public Type StateType { get; set; }
    }
}