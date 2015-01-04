using System;
using System.Linq;
using System.Collections.Generic;
using LoggingWebSite;
using Microsoft.AspNet.WebUtilities.Collections;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public static class LoggingExtensions
    {
        private const string RequestTraceIdKey = "RequestTraceId";

        public static IEnumerable<MessageNode> GetMessageNodes(this IEnumerable<LogNode> logEntries)
        {
            List<MessageNode> messageNodes = new List<MessageNode>();

            foreach (var logEntry in logEntries)
            {
                GetMessages(logEntry, messageNodes);
            }

            return messageNodes;
        }

        public static IEnumerable<MessageNode> GetStartupMessageNodes(this IEnumerable<LogNode> logEntries)
        {
            logEntries = logEntries.Where(entry => entry.RequestInfo != null);

            return logEntries.GetMessageNodes();
        }

        public static IEnumerable<MessageNode> GetMessageNodesWithRequestTraceId(this IEnumerable<LogNode> logEntries, 
                                                                            Guid requestTraceId)
        {
            logEntries = logEntries.Where(entry => entry.RequestInfo != null
                                            && string.Equals(entry.RequestInfo.Query.GetQueryValue(RequestTraceIdKey),
                                               requestTraceId.ToString(), 
                                               StringComparison.OrdinalIgnoreCase));

            return logEntries.GetMessageNodes();
        }

        public static IEnumerable<MessageNode> GetMessagesUnderScope(this IEnumerable<LogNode> logEntries, string scopeName)
        {
            // get list of scopes from the given list of log entries
            List<ScopeNode> scopeNodes = new List<ScopeNode>();
            foreach(var entry in logEntries)
            {
                GetScopes(entry, scopeNodes);
            }

            //TODO: node state if non-string?
            var scopeNode = scopeNodes.FirstOrDefault(node => string.Equals(node.State?.ToString(), scopeName));

            List<MessageNode> messageNodes = new List<MessageNode>();

            // get all messages under the found scope node
            GetMessages(scopeNode, messageNodes);

            return messageNodes;
        }
        

        private static void GetMessages(LogNode node, List<MessageNode> messageNodes)
        {
            var messageNode = node as MessageNode;
            if (messageNode != null)
            {
                messageNodes.Add(messageNode);
                return;
            }

            foreach (var childNode in ((ScopeNode)node).Children)
            {
                GetMessages(childNode, messageNodes);
            }
        }

        private static void GetScopes(LogNode node, List<ScopeNode> scopeNodes)
        {
            var scopeNode = node as ScopeNode;
            if(scopeNode == null)
            {
                return;
            }

            scopeNodes.Add(scopeNode);

            foreach (var childNode in scopeNode.Children)
            {
                GetScopes(childNode, scopeNodes);
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