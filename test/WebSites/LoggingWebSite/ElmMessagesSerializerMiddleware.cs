using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics.Elm;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.WebUtilities;
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
        private const string RequestTraceIdKey = "RequestTraceId";
        private const string StartupLogsKey = "StartupLogs";

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

            // filter to get only startup logs
            if(context.Request.Query.ContainsKey(StartupLogsKey))
            {
                activities = activities.Where(activityContext =>
                {
                    if (activityContext.HttpInfo == null || activityContext.HttpInfo.RequestID == Guid.Empty)
                    {
                        return true;
                    }

                    return false;
                });
            }
            // filter by client's request trace id
            else if (context.Request.Query.ContainsKey(RequestTraceIdKey))
            {
                var clientRequestTraceId = context.Request.Query[RequestTraceIdKey];

                if (!string.IsNullOrWhiteSpace(clientRequestTraceId))
                {
                    activities = activities.Where(activityContext =>
                    {
                        if(activityContext.HttpInfo != null && activityContext.HttpInfo.Query.HasValue)
                        {
                            var queryString = QueryHelpers.ParseQuery(activityContext.HttpInfo.Query.Value);

                            if(queryString.ContainsKey(RequestTraceIdKey) &&
                                queryString[RequestTraceIdKey].Equals(clientRequestTraceId, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
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
                RequestInfo = GetRequestInfo(logInfo.ActivityContext?.HttpInfo),
                EventID = logInfo.EventID,
                Message = logInfo.Message,
                LoggerName = logInfo.Name,
                Severity = logInfo.Severity,
                State = logInfo.State,
                Time = logInfo.Time
            });
        }

        private HttpRequestInfo GetRequestInfo(HttpInfo httpInfo)
        {
            if (httpInfo == null) return null;

            var requestInfo = new HttpRequestInfo();
            requestInfo.RequestID = httpInfo.RequestID;
            requestInfo.StatusCode = httpInfo.StatusCode;
            requestInfo.Scheme = httpInfo.Scheme;
            requestInfo.Query = httpInfo.Query.Value;
            requestInfo.Protocol = httpInfo.Protocol;
            requestInfo.Path = httpInfo.Path.Value;
            requestInfo.Method = httpInfo.Method;
            requestInfo.Host = httpInfo.Host.Value;
            requestInfo.Headers = httpInfo.Headers.ToList();
            requestInfo.Cookies = httpInfo.Cookies.ToList();
            requestInfo.ContentType = httpInfo.ContentType;

            return requestInfo;
        }
    }
}