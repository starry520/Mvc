using System;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Mvc
{
    internal static class ParsingHelpers
    {
        public static long? GetContentLength(IHeaderDictionary headers)
        {
            var value = headers["Content-Length"];

            if (value == null)
            {
                return null;
            }
            else
            {
                return Convert.ToInt64(value);
            }
        }
    }
}