// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Mvc
{
    // This class gets generated only at result execution time 
    // (at the execution phase of the result filter)
    public class HttpActionResult
    {
        private readonly ActionResultContext _resultContext;
        private readonly IActionResult _innerResult;

        public HttpActionResult([NotNull] IActionResult innerResult) : this()
        {
            _innerResult = innerResult;
        }

        public HttpActionResult()
        {
            _resultContext = new ActionResultContext();
            PopulateHeaders(_resultContext);
        }

        public virtual async Task ExecuteResultAsync(ActionContext context)
        {
            _innerResult.PopulateHeaders(_resultContext);

            ApplyHeaders(context);

            // first invoke the inner result then this one so the order of execution is predictable.
            // the common case will be that only one of the invokes is populated with code.
            await _innerResult?.Invoke(context.HttpContext.Response.Body);
            await Invoke(context.HttpContext.Response.Body);
        }

        public virtual Task Invoke(Stream bodyStream)
        {
            return Task.FromResult(true);
        }

        private void ApplyHeaders(ActionContext context)
        {
            var response = context.HttpContext.Response;

            foreach (var header in _resultContext?.CreateOnReadWriteHeaders)
            {
                response.Headers.AppendValues(header.Key, header.Value);
            }

            // content length and content type will both be backed by the header collection
            // so the following two assignments should go away
            if (_resultContext.ContentLength != null)
            {
                response.ContentLength = _resultContext.ContentLength;
            }

            response.ContentType = _resultContext.ContentType;

            if (_resultContext.StatusCode.HasValue)
            {
                response.StatusCode = _resultContext.StatusCode.Value;
            }
        }
    }
}
