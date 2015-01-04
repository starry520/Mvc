using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;

namespace LoggingWebSite
{
    public class LogCaptureMiddleware
    {
        private readonly RequestDelegate _next;
        
        public LogCaptureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestInfo = BuildRequestInfo(context.Request, requestId: Guid.NewGuid());
            RequestContext.Current = new RequestContext(requestInfo);

            await _next(context);
        }

        private static HttpRequestInfo BuildRequestInfo(HttpRequest request, Guid requestId)
        {
            return new HttpRequestInfo()
            {
                RequestID = requestId,
                Host = request.Host.Value,
                ContentType = request.ContentType,
                Path = request.Path.Value,
                Scheme = request.Scheme,
                Method = request.Method,
                Protocol = request.Protocol,
                Headers = request.Headers.ToArray(),
                Query = request.Query.ToArray(),
                Cookies = request.Cookies.ToArray()
            };
        }
    }
}