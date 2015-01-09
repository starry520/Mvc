// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.HeaderValueAbstractions;

namespace Microsoft.AspNet.Mvc
{
    public class ConsumesActionFilterAttribute : AuthorizationFilterAttribute
    {
        public IList<MediaTypeHeaderValue> ContentTypes { get; set; }
        public ConsumesActionFilterAttribute(string contentType, params string[] otherContentTypes)
        {
            ContentTypes = GetContentTypes(contentType, otherContentTypes);
        }

        public override async Task OnAuthorizationAsync([NotNull] AuthorizationContext context)
        {
            await base.OnAuthorizationAsync(context);
            var requestContentType = MediaTypeHeaderValue.Parse(context.HttpContext.Request.ContentType);
            if (!ContentTypes.Any(contentType => contentType.IsSubsetOf(requestContentType)))
            {
                // short circut
                context.Result = new UnsupportedMediaTypeResult();
            }
        }

        private List<MediaTypeHeaderValue> GetContentTypes(string firstArg, string[] args)
        {
            var contentTypes = new List<MediaTypeHeaderValue>();
            contentTypes.Add(MediaTypeHeaderValue.Parse(firstArg));
            foreach (var item in args)
            {
                var contentType = MediaTypeHeaderValue.Parse(item);
                contentTypes.Add(contentType);
            }

            return contentTypes;
        }
    }
}