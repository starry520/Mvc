// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class LoggingTests
    {
        private readonly IServiceProvider _provider = TestHelper.CreateServices(nameof(LoggingWebSite));
        private readonly Action<IApplicationBuilder> _app = new Startup().Configure;
        private const string ClientRequestTraceIdHeader = "ClientRequestTraceId";

        [Fact]
        public async Task StartupLogsTest()
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();
            var clientRequestTraceId = Guid.NewGuid().ToString();
            client.DefaultRequestHeaders.Add(ClientRequestTraceIdHeader, clientRequestTraceId);
            var expectedLogMessages = new List<LogMessage>();

            // Act
            var response = await client.GetAsync("http://localhost/Home/Index");
            var logResponse = await client.GetAsync("http://localhost/elm-messages");

            var data = await logResponse.Content.ReadAsStringAsync();
            var logMessages = JsonConvert.DeserializeObject<List<LogMessage>>(data);

            // Assert
        }

        [Fact]
        public async Task RouteLogsTest()
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();
            var clientRequestTraceId = Guid.NewGuid().ToString();
            client.DefaultRequestHeaders.Add(ClientRequestTraceIdHeader, clientRequestTraceId);
            var expectedLogMessages = new List<LogMessage>();

            // Arrange & Act
            var response = await client.GetAsync("http://localhost/Home/Index");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = await response.Content.ReadAsStringAsync();
            Assert.Equal("Home.Index", data);

            var logResponse = await client.GetAsync("http://localhost/elm-messages");
            data = await logResponse.Content.ReadAsStringAsync();
            var logMessages = JsonConvert.DeserializeObject<List<LogMessage>>(data);

            // Assert
        }
    }

    public class LogMessage
    {
        public int EventID { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public LogLevel Severity { get; set; }
        public object State { get; set; }
        public DateTimeOffset Time { get; set; }
        public HttpInfo HttpInfo { get; set; }
    }
}