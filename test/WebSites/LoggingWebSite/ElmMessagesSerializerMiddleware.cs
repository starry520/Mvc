using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics.Elm;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LoggingWebSite
{
    public class ElmMessagesSerializerMiddleware
    {
        private readonly RequestDelegate nextMiddleware;
        private List<LogMessage> _logMessages = null;
        private const string ClientRequestTraceIdHeader = "ClientRequestTraceId";

        public ElmMessagesSerializerMiddleware(RequestDelegate nextMiddleware)
        {
            this.nextMiddleware = nextMiddleware;
        }

        public async Task Invoke(HttpContext context)
        {
            // log messages DTOs that are serialized to the client
            _logMessages = new List<LogMessage>();

            var elmStore = context.RequestServices.GetService<ElmStore>();
            var activities = elmStore.GetActivities();

            // Filter by client's request id trace header
            string[] values = null;
            string clientRequestTraceId;
            if (context.Request.Headers.TryGetValue(ClientRequestTraceIdHeader, out values))
            {
                clientRequestTraceId = values.First();

                if (!string.IsNullOrWhiteSpace(clientRequestTraceId))
                {
                    activities = activities.Where(activityContext =>
                    {
                        string[] headerValues = null;
                        if (activityContext.HttpInfo != null
                            && activityContext.HttpInfo.Headers != null
                            && activityContext.HttpInfo.Headers.TryGetValue(ClientRequestTraceIdHeader, out headerValues))
                        {
                            return headerValues.First().Equals(clientRequestTraceId, StringComparison.OrdinalIgnoreCase);
                        }

                        return false;
                    });
                }
            }

            // Build a flat list of log messages from the log node tree 
            // in order to be serialized to the client
            foreach (var activity in activities.Reverse())
            {
                if (!activity.RepresentsScope)
                {
                    // message not within a scope
                    var logInfo = activity.Root.Messages.FirstOrDefault();
                    AddMessage(logInfo);
                }
                else
                {
                    Traverse(activity.Root);
                }
            }

            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";

            using (var sw = new StreamWriter(context.Response.Body, Encoding.UTF8, 1024, leaveOpen: true))
            {
                var data = JsonConvert.SerializeObject(_logMessages, Formatting.Indented, new StringEnumConverter());

                await sw.WriteAsync(data);
            }
        }

        private void Traverse(ScopeNode node)
        {
            // print start of scope
            AddMessage(new LogInfo()
            {
                Name = node.Name,
                Time = node.StartTime,
                Severity = LogLevel.Verbose,
                Message = "Begin-Scope:" + node.State
            });

            var list = new List<object>();
            list.AddRange(node.Messages);
            list.AddRange(node.Children);
            list.Sort(new LogTimeComparer());

            foreach (var obj in list)
            {
                // check if the current node is a regular message node
                // or a scope node
                var logInfo = obj as LogInfo;

                if (logInfo != null)
                {
                    AddMessage(logInfo);
                }
                else
                {
                    Traverse((ScopeNode)obj);
                }
            }

            // print end of scope
            AddMessage(new LogInfo()
            {
                Name = node.Name,
                Time = node.EndTime,
                Severity = LogLevel.Verbose,
                Message = "End-Scope:" + node.State
            });
        }

        private void AddMessage(LogInfo logInfo)
        {
            _logMessages.Add(new LogMessage()
            {
                HttpInfo = logInfo.ActivityContext?.HttpInfo,
                EventID = logInfo.EventID,
                Message = logInfo.Message,
                Name = logInfo.Name,
                Severity = logInfo.Severity,
                State = logInfo.State,
                Time = logInfo.Time
            });
        }
    }
}