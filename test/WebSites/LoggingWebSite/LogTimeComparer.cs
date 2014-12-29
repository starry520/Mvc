using System;
using System.Collections.Generic;
using Microsoft.AspNet.Diagnostics.Elm;

namespace LoggingWebSite
{
    public class LogTimeComparer : IComparer<object>
    {
        public int Compare(object x, object y)
        {
            LogInfo xLogInfo = x as LogInfo;
            ScopeNode xScopeNode = x as ScopeNode;

            LogInfo yLogInfo = y as LogInfo;
            ScopeNode yScopeNode = y as ScopeNode;

            if (xLogInfo != null)
            {
                if (yLogInfo != null)
                {
                    return xLogInfo.Time.CompareTo(yLogInfo.Time);
                }
                else
                {
                    return xLogInfo.Time.CompareTo(yScopeNode.StartTime);
                }
            }
            else
            {
                if (yScopeNode != null)
                {
                    return xScopeNode.StartTime.CompareTo(yScopeNode.StartTime);
                }
                else
                {
                    return xScopeNode.StartTime.CompareTo(yLogInfo.Time);
                }
            }
        }
    }
}