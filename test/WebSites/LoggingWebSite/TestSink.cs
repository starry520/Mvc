using System;
using System.Collections.Generic;

namespace LoggingWebSite
{
    public class TestSink
    {
        public List<LogNode> LogEntries { get; set; } = new List<LogNode>();

        /// <summary>
        /// Flat list of scope nodes.
        /// This list is for enabling faster search of scopes. 
        /// For example: "Get all messages under scope 'blah'"
        /// </summary>
        public List<ScopeNode> ScopeNodes { get; set; } = new List<ScopeNode>();
    }
}