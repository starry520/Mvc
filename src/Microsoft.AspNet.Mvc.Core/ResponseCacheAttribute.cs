// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// An <see cref="ActionFilterAttribute"/> which sets the appropriate headers related to Response caching.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ResponseCacheAttribute : ActionFilterAttribute
    {
        // A nullable-int cannot be used as an Attribute parameter.
        // Hence this nullable-int is present to back the Duration property.
        private int? _duration;
        
        /// <summary>
        /// Gets or sets the duration in seconds for which the response is cached.
        /// </summary>
        public int Duration
        { 
            get
            {
                if (_duration == null)
                {
                    return 0;
                }

                return (int)_duration;
            }
            set
            {
                _duration = value;
            }
        }

        /// <summary>
        /// Gets or sets the location where the data from a particular URL must be cached.
        /// </summary>
        public ResponseCacheLocation Location { get; set; }

        /// <summary>
        /// Gets or sets the value which determines whether the data should be stored or not.
        /// </summary>
        public bool NoStore { get; set; }

        /// <summary>
        /// Gets or sets the value for the Vary response header.
        /// </summary>
        public string VaryByHeader { get; set; }

        // <inheritdoc />
        public override void OnActionExecuting([NotNull] ActionExecutingContext context)
        {
            var headers = context.HttpContext.Response.Headers;

            if (!string.IsNullOrEmpty(VaryByHeader))
            {
                headers.Set("Vary", VaryByHeader);
            }

            if (NoStore)
            {
                headers.Set("Cache-control", "no-store");
                // Cache-control: no-store, no-cache is valid.
                if (Location == ResponseCacheLocation.None)
                {
                    headers.Append("Cache-control", "no-cache");
                    headers.Set("Pragma", "no-cache");
                }
            }
            else
            {
                string cacheControlValue = null;
                switch (Location)
                {
                    case ResponseCacheLocation.Any:
                        cacheControlValue = "public";
                        break;
                    case ResponseCacheLocation.Client:
                        cacheControlValue = "private";
                        break;
                    case ResponseCacheLocation.None:
                        cacheControlValue = "no-cache";
                        headers.Set("Pragma", "no-cache");
                        break;
                }

                if (_duration != null)
                {
                    cacheControlValue = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}{1}max-age={2}",
                        cacheControlValue,
                        cacheControlValue != null? "," : null,
                        Duration.ToString());
                }

                headers.Set("Cache-control", cacheControlValue);
            }
        }
    }
}