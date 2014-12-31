using System;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Logging;

namespace LoggingWebSite
{
    public class LogOptions
    {
        /// <summary>
        /// Determines whether log statements should be logged based on the name of the logger
        /// and the <see cref="LogLevel"/> of the message.
        /// </summary>
        public Func<string, LogLevel, bool> Filter { get; set; } = (name, level) => true;
    }
}