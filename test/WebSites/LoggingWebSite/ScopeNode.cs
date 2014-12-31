using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LoggingWebSite
{
    public class ScopeNode : LogNode
    {
        public List<LogNode> Children { get; set; } = new List<LogNode>();
    }
}