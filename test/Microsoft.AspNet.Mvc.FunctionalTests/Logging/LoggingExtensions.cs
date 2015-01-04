using System;
using System.Linq;
using System.Collections.Generic;
using LoggingWebSite;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public static class LoggingExtensions
    {
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
        public static IEnumerable<MessageNode> GetMessagesUnderScope(this IEnumerable<ScopeNode> scopeNodes, 
                                                                    string scopeName)
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