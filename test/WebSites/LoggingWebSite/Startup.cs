// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;
using Microsoft.AspNet.Diagnostics.Elm;
using System.Collections.Generic;
using Microsoft.Framework.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Text;

namespace LoggingWebSite
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            var configuration = app.GetTestConfiguration();

            // Set up application services
            app.UseServices(services =>
            {
                services.AddSingleton<ILoggerFactory, LoggerFactory>();

                services.AddElm();

                // Add MVC services to the services container
                services.AddMvc(configuration);
            });

            // serializes log messages from ElmStore into json format to the client
            app.Map(new PathString("/elm-messages"), (appBuilder) =>
            {
                appBuilder.UseMiddleware<ElmMessagesSerializerMiddleware>();
            });

            app.UseElmCapture();

            // Add MVC to the request pipeline
            app.UseMvc(routes =>
            {
                routes.MapRoute("Default", "{controller=Home}/{action=Index}");
            });
        }
    }

    public class CustomElmStore : ElmStore
    {
        
    }
}
