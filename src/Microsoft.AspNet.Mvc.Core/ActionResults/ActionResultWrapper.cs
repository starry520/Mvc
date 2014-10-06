using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Mvc.Core
{

    public class ActionResultWrapper : IActionResult
    {
        public IActionResult InnerResult { get; private set; }

        private readonly Action<ActionResultContext> _action;

        public ActionResultWrapper(IActionResult actionResult, Action<ActionResultContext> action)
        {
            InnerResult = actionResult;
        }

        public void PopulateHeaders(ActionResultContext context)
        {
            InnerResult.PopulateHeaders(context);
            _action(context);
        }

        public async Task Invoke(Stream bodyStream)
        {
            await InnerResult.Invoke(bodyStream);
        }
    }
}