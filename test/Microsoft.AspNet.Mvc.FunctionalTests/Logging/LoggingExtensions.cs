using System;
using System.Linq;
using System.Collections.Generic;
using LoggingWebSite;
using Microsoft.AspNet.WebUtilities.Collections;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Gets ALL message nodes in the order that they are logged
        /// </summary>
        /// <param name="logEntries"></param>
        /// <returns></returns>
        public static IEnumerable<MessageNode> GetAllMessages(this IEnumerable<LogNode> logEntries)
        {
            List<MessageNode> messageNodes = new List<MessageNode>();

            foreach (var logEntry in logEntries)
            {
                Traverse(logEntry, messageNodes);
            }

            return messageNodes;
        }

        /// <summary>
        /// Gets ALL message nodes with the given request trace Id and in the order they are logged.
        /// </summary>
        /// <param name="logEntries"></param>
        /// <param name="requestTraceId"></param>
        /// <returns></returns>
        public static IEnumerable<MessageNode> GetMessagesOfRequestTraceId(this IEnumerable<LogNode> logEntries, 
                                                                            Guid requestTraceId)
        {
            return logEntries.GetAllMessages()
                             .Where(entry => entry.RequestInfo != null
                                            && string.Equals(entry.RequestInfo.Query.GetQueryValue("RequestTraceId"),
                                               requestTraceId.ToString(), 
                                               StringComparison.OrdinalIgnoreCase));
        }

        //TODO: force GetMessagesUnderScope to only apply for ScopeNodes

        public static IEnumerable<MessageNode> GetMessagesUnderScope(this IEnumerable<LogNode> logEntries,
                                                                        Guid requestTraceId, string scopeName)
        {
            logEntries = logEntries.Where(entry => entry.RequestInfo != null
                                                    && entry.RequestInfo.RequestID.Equals(requestTraceId));

            List<ScopeNode> scopeNodes = new List<ScopeNode>();
            foreach(var entry in logEntries)
            {
                GetAllScopes(entry, scopeNodes);
            }

            var scopeNode = scopeNodes.FirstOrDefault(node => string.Equals(node.State?.ToString(), scopeName));

            List<MessageNode> messageNodes = new List<MessageNode>();

            Traverse(scopeNode, messageNodes);

            return messageNodes;
        }

        ///// <summary>
        ///// Gets all the message nodes under a supplied scope
        ///// </summary>
        ///// <param name="scopeNodes"></param>
        ///// <param name="scopeName"></param>
        ///// <returns></returns>
        //public static IEnumerable<MessageNode> GetMessagesUnderScope(this IEnumerable<ScopeNode> scopeNodes, 
        //                                                            string scopeName)
        //{
        //    //TODO: what if state is null
        //    var scopeNode = scopeNodes.Where(node => string.Equals(node.State?.ToString(), scopeName))
        //                              .FirstOrDefault();
        //    if (scopeNode == null)
        //    {
        //        throw new Exception(string.Format("No scope with name '{0}' was found.", scopeName));
        //    }

        //    List<MessageNode> messageNodes = new List<MessageNode>();

        //    Traverse(scopeNode, messageNodes);

        //    return messageNodes;
        //}

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

        private static void GetAllScopes(LogNode node, List<ScopeNode> scopeNodes)
        {
            var scopeNode = node as ScopeNode;
            if(scopeNode == null)
            {
                return;
            }

            scopeNodes.Add(scopeNode);

            foreach (var childNode in scopeNode.Children)
            {
                GetAllScopes(childNode, scopeNodes);
            }
        }

        private static string GetQueryValue(this IEnumerable<KeyValuePair<string, string[]>> query, string key)
        {
            var queryKeyValues = query.Where(kvp => string.Equals(kvp.Key, key, StringComparison.OrdinalIgnoreCase))
                                    .Select(kvp => kvp.Value).FirstOrDefault();

            if(queryKeyValues != null && queryKeyValues.Count() > 0)
            {
                return queryKeyValues.First();
            }

            return null;
        }
    }
}