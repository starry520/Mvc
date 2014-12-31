using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using System;

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

        /// <summary>
        /// Gets all message nodes in the order that they are logged
        /// </summary>
        /// <param name="sink"></param>
        /// <returns></returns>
        public static IEnumerable<MessageNode> GetMessages(this TestSink sink)
        {
            return sink.LogEntries.GetMessages();
        }

        /// <summary>
        /// Gets all the message nodes under a supplied scope
        /// </summary>
        /// <param name="sink"></param>
        /// <param name="scopeName"></param>
        /// <returns></returns>
        public static IEnumerable<MessageNode> GetMessagesUnderScope(this TestSink sink, string scopeName)
        {
            return sink.ScopeNodes.GetMessagesUnderScope(scopeName);
        }

        /// <summary>
        /// Gets all message nodes in the order that they are logged
        /// </summary>
        /// <param name="logEntries"></param>
        /// <returns></returns>
        public static IEnumerable<MessageNode> GetMessages(this IEnumerable<LogNode> logEntries)
        {
            List<MessageNode> messageNodes = new List<MessageNode>();

            foreach (var logEntry in logEntries)
            {
                Traverse(logEntry, messageNodes);
            }

            return messageNodes;
        }

        //TODO: force GetMessagesUnderScope to only apply for ScopeNodes

        /// <summary>
        /// Gets all the message nodes under a supplied scope
        /// </summary>
        /// <param name="scopeNodes"></param>
        /// <param name="scopeName"></param>
        /// <returns></returns>
        public static IEnumerable<MessageNode> GetMessagesUnderScope(this IEnumerable<ScopeNode> scopeNodes, string scopeName)
        {
            //TODO: what if state is null
            var scopeNode = scopeNodes.Where(node => string.Equals(node.State?.ToString(), scopeName))
                                      .FirstOrDefault();
            if (scopeNode == null)
            {
                throw new Exception(string.Format("No scope with name '{0}' was found.", scopeName));
            }

            List<MessageNode> messageNodes = new List<MessageNode>();

            Traverse(scopeNode, messageNodes);

            return messageNodes;
        }

        private static void Traverse(LogNode node, List<MessageNode> messageNodes)
        {
            var messageNode = node as MessageNode;
            if (messageNode != null)
            {
                messageNodes.Add(messageNode);
                return;
            }

            foreach (var childNode in ((ScopeNode)node).Children)
            {
                Traverse(childNode, messageNodes);
            }
        }
    }
}