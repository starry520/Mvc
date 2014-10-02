// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Mvc
{
    public abstract class HttpActionResult : IHttpActionResult
    {
        private readonly ActionResultContext _resultContext;

        public int? StatusCode
        {
            get { return _resultContext.StatusCode; }
            set { _resultContext.StatusCode = value; }
        }

        public long? ContentLength
        {
            get { return _resultContext.ContentLength; }
            set { _resultContext.ContentLength = value; }
        }

        public IHeaderDictionary Headers
        {
            get { return _resultContext.Headers; }
        }

        public string ContentType
        {
            get { return _resultContext.ContentType; }
            set { _resultContext.ContentType = value; }
        }

        private readonly IActionResult _innerResult;

        public IActionResult InnerResult { get { return _innerResult; } }

        public HttpActionResult([NotNull] IActionResult innerResult) : this()
        {
            _innerResult = innerResult;
            innerResult.PopulateHeaders(_resultContext);
        }

        public HttpActionResult()
        {
            _resultContext = new ActionResultContext();
            PopulateHeaders(_resultContext);
        }

        public virtual async Task ExecuteResultAsync(ActionContext context)
        {
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

        public virtual void PopulateHeaders(ActionResultContext context)
        {
        }

        private void ApplyHeaders(ActionContext context)
        {
            var response = context.HttpContext.Response;

            foreach (var header in _resultContext?.LazyHeaders)
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
