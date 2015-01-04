// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LoggingWebSite;
using LoggingWebSite.Controllers;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc.Logging;
using Microsoft.AspNet.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class LoggingTests
    {
        private readonly IServiceProvider _serviceProvider = TestHelper.CreateServices("LoggingWebSite");
        private readonly Action<IApplicationBuilder> _app = new LoggingWebSite.Startup().Configure;

        [Fact]
        public async Task AssemblyValues_LoggedAtStartup()
        {
            // Arrange and Act
            var logEntries = await GetStartupLogs();
            logEntries = logEntries.Where(entry => entry.StateType.Equals(typeof(AssemblyValues)));

            Assert.NotEmpty(logEntries);
            foreach (var entry in logEntries)
            {
                dynamic assembly = entry.State;
                Assert.NotNull(assembly);
                Assert.Equal(
                    "LoggingWebSite, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    assembly.AssemblyName.ToString());
            }
        }

        [Fact]
        public async Task IsControllerValues_LoggedAtStartup()
        {
            // Arrange and Act
            var logEntries = await GetStartupLogs();
            logEntries = logEntries.Where(entry => entry.StateType.Equals(typeof(IsControllerValues)));

            // Assert
            Assert.NotEmpty(logEntries);
            foreach (var entry in logEntries)
            {
                dynamic isController = entry.State;
                if (string.Equals(typeof(HomeController).AssemblyQualifiedName, isController.Type.ToString()))
                {
                    Assert.Equal(
                        ControllerStatus.IsController,
                        Enum.Parse(typeof(ControllerStatus), isController.Status.ToString()));
                }
                else
                {
                    Assert.NotEqual(ControllerStatus.IsController,
                        Enum.Parse(typeof(ControllerStatus), isController.Status.ToString()));
                }
            }
        }

        [Fact]
        public async Task ControllerModelValues_LoggedAtStartup()
        {
            // Arrange and Act
            var logEntries = await GetStartupLogs();
            logEntries = logEntries.Where(entry => entry.StateType.Equals(typeof(ControllerModelValues)));

            // Assert
            Assert.Single(logEntries);
            dynamic controller = logEntries.First().State;
            Assert.Equal("Home", controller.ControllerName.ToString());
            Assert.Equal(typeof(HomeController).AssemblyQualifiedName, controller.ControllerType.ToString());
            Assert.Equal("Index", controller.Actions[0].ActionName.ToString());
            Assert.Empty(controller.ApiExplorer.IsVisible);
            Assert.Empty(controller.ApiExplorer.GroupName.ToString());
            Assert.Empty(controller.Attributes);
            Assert.Empty(controller.Filters);
            Assert.Empty(controller.ActionConstraints);
            Assert.Empty(controller.RouteConstraints);
            Assert.Empty(controller.AttributeRoutes);
        }

        [Fact]
        public async Task ActionDescriptorValues_LoggedAtStartup()
        {
            // Arrange and Act
            var logEntries = await GetStartupLogs();
            logEntries = logEntries.Where(entry => entry.StateType.Equals(typeof(ActionDescriptorValues)));

            // Assert
            Assert.Single(logEntries);
            dynamic action = logEntries.First().State;
            Assert.Equal("Index", action.Name.ToString());
            Assert.Empty(action.Parameters);
            Assert.Empty(action.FilterDescriptors);
            Assert.Equal("controller", action.RouteConstraints[0].RouteKey.ToString());
            Assert.Equal("action", action.RouteConstraints[1].RouteKey.ToString());
            Assert.Empty(action.RouteValueDefaults);
            Assert.Empty(action.ActionConstraints.ToString());
            Assert.Empty(action.HttpMethods.ToString());
            Assert.Empty(action.Properties);
            Assert.Equal("Home", action.ControllerName.ToString());
        }

        private async Task<IEnumerable<MessageNode>> GetStartupLogs()
        {
            // Arrange
            var server = TestServer.Create(_serviceProvider, _app);
            var client = server.CreateClient();

            // Act
            // regular request to kick start log generation
            var response = await client.GetAsync("http://localhost/Home/Index");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var data = await response.Content.ReadAsStringAsync();
            Assert.Equal("Home.Index", data);

            // get all logs from the sink
            data = await client.GetStringAsync("http://localhost/logs");

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Insert(0, new LogNodeJsonConverter());
            
            var allLogEntries = JsonConvert.DeserializeObject<IEnumerable<LogNode>>(data, 
                                                                                    serializerSettings);

            // get a flattened list of message nodes without the scoping nodes information.
            var messageLogs = allLogEntries.GetStartupMessageNodes();

            return messageLogs;
        }
    }
}