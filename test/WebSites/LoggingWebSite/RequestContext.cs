#if ASPNET50 || ASPNETCORE50
#if ASPNET50
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
#endif
using System;
using System.Threading;

namespace LoggingWebSite
{
    public class RequestContext
    {
        public RequestContext(HttpRequestInfo requestInfo)
        {
            RequestInfo = requestInfo;
            Current = this;
        }

        public HttpRequestInfo RequestInfo { get; private set; }

#if ASPNET50
        private static string FieldKey = typeof(RequestContext).FullName + ".Value";
        public static RequestContext Current
        {
            get
            {
                var handle = CallContext.LogicalGetData(FieldKey) as ObjectHandle;

                if (handle == null)
                {
                    return default(RequestContext);
                }

                return (RequestContext)handle.Unwrap();
            }
            set
            {
                CallContext.LogicalSetData(FieldKey, new ObjectHandle(value));
            }
        }
#else
        private static AsyncLocal<RequestContext> _value = new AsyncLocal<RequestContext>();
        public static RequestContext Current
        {
            set
            {
                _value.Value = value;
            }
            get
            {
                return _value.Value;
            }
        }
#endif
    }
}
#endif