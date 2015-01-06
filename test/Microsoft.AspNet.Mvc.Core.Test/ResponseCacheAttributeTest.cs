// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNet.PipelineCore;
using Microsoft.AspNet.Routing;
using Xunit;

namespace Microsoft.AspNet.Mvc
{
    public class ResponseCacheAttributeTest
    {
        public static IEnumerable<object[]> CacheControlData
        {
            get
            {
                yield return new object[] { new ResponseCacheAttribute { NoStore = true }, "no-store" };
                // If no-store is set, then location cannot be set.
                yield return new object[] {
                    new ResponseCacheAttribute { NoStore = true, Location = ResponseCacheLocation.Client },
                    "no-store"
                };
                yield return new object[] {
                    new ResponseCacheAttribute { NoStore = true, Location = ResponseCacheLocation.Any },
                    "no-store"
                };
                // If no-store is set, then duration cannot be set.
                yield return new object[] {
                    new ResponseCacheAttribute { NoStore = true, Duration = 100 }, "no-store"
                };

                yield return new object[] {
                    new ResponseCacheAttribute { Location = ResponseCacheLocation.Client }, "private"
                };
                yield return new object[] {
                    new ResponseCacheAttribute { Location = ResponseCacheLocation.Any }, "public"
                };
                yield return new object[] {
                    new ResponseCacheAttribute { Location = ResponseCacheLocation.None }, "no-cache"
                };
                yield return new object[] {
                    new ResponseCacheAttribute { Location = ResponseCacheLocation.Client, Duration=10 },
                    "private,max-age=10"
                };
                yield return new object[] {
                    new ResponseCacheAttribute { Location = ResponseCacheLocation.Any, Duration = 31536000 },
                    "public,max-age=31536000"
                };
                yield return new object[] {
                    new ResponseCacheAttribute { Duration = 20 },
                    "public,max-age=20"
                };
            }
        }

        [Theory]
        [MemberData(nameof(CacheControlData))]
        public void OnActionExecuting_CanSetCacheControlHeaders(ResponseCacheAttribute cache, string output)
        {
            // Arrange
            var context = GetActionExecutingContext();

            // Act
            cache.OnActionExecuting(context);

            // Assert
            Assert.Equal(output, context.HttpContext.Response.Headers.Get("Cache-control"));
        }

        public static IEnumerable<object[]> NoStoreData
        {
            get
            {
                // If no-store is set, then location cannot be set.
                yield return new object[] {
                    new ResponseCacheAttribute { NoStore = true, Location = ResponseCacheLocation.Client },
                    "no-store"
                };
                yield return new object[] {
                    new ResponseCacheAttribute { NoStore = true, Location = ResponseCacheLocation.Any },
                    "no-store"
                };
                // If no-store is set, then duration cannot be set.
                yield return new object[] {
                    new ResponseCacheAttribute { NoStore = true, Duration = 100 }, "no-store"
                };
            }
        }

        [Theory]
        [MemberData(nameof(NoStoreData))]
        public void OnActionExecuting_DoesNotSetLocationOrDuration_IfNoStoreIsSet(ResponseCacheAttribute cache, string output)
        {
            // Arrange
            var context = GetActionExecutingContext();

            // Act
            cache.OnActionExecuting(context);

            // Assert
            Assert.Equal(output, context.HttpContext.Response.Headers.Get("Cache-control"));
        }

        public static IEnumerable<object[]> VaryData
        {
            get
            {
                yield return new object[] { new ResponseCacheAttribute { VaryByHeader = "Accept" }, "Accept", "public" };
                yield return new object[] {
                    new ResponseCacheAttribute { VaryByHeader = "Accept", NoStore = true },
                    "Accept",
                    "no-store"
                };
                yield return new object[] {
                    new ResponseCacheAttribute { Location = ResponseCacheLocation.Client, VaryByHeader = "Accept" },
                    "Accept",
                    "private"
                };
                yield return new object[] {
                    new ResponseCacheAttribute { Location = ResponseCacheLocation.Any, VaryByHeader = "Test" },
                    "Test",
                    "public"
                };
                yield return new object[] {
                    new ResponseCacheAttribute {
                        Location = ResponseCacheLocation.Client, Duration=10, VaryByHeader = "Test"
                    },
                    "Test",
                    "private,max-age=10"
                };
                yield return new object[] {
                    new ResponseCacheAttribute {
                        Location = ResponseCacheLocation.Any,
                        Duration = 31536000,
                        VaryByHeader = "Test"
                    },
                    "Test",
                    "public,max-age=31536000"
                };
            }
        }

        [Theory]
        [MemberData(nameof(VaryData))]
        public void ResponseCacheCanSetVary(ResponseCacheAttribute cache, string varyOutput, string cacheControlOutput)
        {
            // Arrange
            var context = GetActionExecutingContext();

            // Act
            cache.OnActionExecuting(context);

            // Assert
            Assert.Equal(varyOutput, context.HttpContext.Response.Headers.Get("Vary"));
            Assert.Equal(cacheControlOutput, context.HttpContext.Response.Headers.Get("Cache-control"));
        }

        [Fact]
        public void SetsPragmaOnNoCache()
        {
            // Arrange
            var context = GetActionExecutingContext();
            var cache = new ResponseCacheAttribute()
                            {
                                NoStore = true,
                                Location = ResponseCacheLocation.None
                            };

            // Act
            cache.OnActionExecuting(context);

            // Assert
            Assert.Equal("no-store,no-cache", context.HttpContext.Response.Headers.Get("Cache-control"));
            Assert.Equal("no-cache", context.HttpContext.Response.Headers.Get("Pragma"));
        }

        private ActionExecutingContext GetActionExecutingContext()
        {
            return new ActionExecutingContext(
                new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                new IFilter[0],
                new Dictionary<string, object>());
        }
    }
}