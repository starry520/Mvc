// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ConnegWebSite;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics.Elm;
using Microsoft.AspNet.TestHost;
using Microsoft.Framework.Logging;
using Newtonsoft.Json;
using Xunit;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Mvc.Logging;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class LoggingTests
    {
        private readonly IServiceProvider _provider = TestHelper.CreateServices("LoggingWebSite");
        private readonly Action<IApplicationBuilder> _app = new LoggingWebSite.Startup().Configure;
        private const string RequestTraceIdKey = "RequestTraceId";
        private const string StartupLogsKey = "StartupLogs";

        [Fact]
        public async Task StartupLogsTest()
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();
            var expectedLogMessages = new List<LogMessage>();

            // Act & Assert

            // regular request
            var response = await client.GetAsync("http://localhost/Home/Index");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = await response.Content.ReadAsStringAsync();
            Assert.Equal("Home.Index", data);

            // request to get startup logs
            response = await client.GetAsync(string.Format("http://localhost/elm-messages?{0}={1}",
                                                    StartupLogsKey, "true"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            data = await response.Content.ReadAsStringAsync();
            var logMessages = JsonConvert.DeserializeObject<IEnumerable<LogMessage>>(data);

            // filter by 'Logger' name
            logMessages = logMessages.Where(msg => 
                            msg.LoggerName == "Microsoft.AspNet.Mvc.ControllerActionDescriptorProvider");

            //Assert.Equal(1, logMessages.Count());
            //var logMessage = logMessages.First();

            //Assert.NotNull(logMessage.State);
            //var controllerModelLog = logMessages.First().State.ToObject<ControllerModelValues>();

            //Assert.Equal("Home", controllerModelLog.ControllerName);
        }
    }

    public class LogMessage
    {
        public int EventID { get; set; }

        public string Message { get; set; }

        public string LoggerName { get; set; }

        public LogLevel Severity { get; set; }

        public JObject State { get; set; }

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

        public List<KeyValuePair<string, string[]>> Headers { get; set; }

        public string Query { get; set; }

        public List<KeyValuePair<string, string[]>> Cookies { get; set; }
    }
}