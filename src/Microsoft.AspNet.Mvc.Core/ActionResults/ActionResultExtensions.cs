using System;

namespace Microsoft.AspNet.Mvc.Core
{
    public static class ActionResultExtensions
    {
        private static bool Always(ActionResultContext context)
        {
            return true;
        }

        private static void AddHeader([NotNull] ActionResultContext context, [NotNull] string key, string value)
        {
            context.Headers[key] = value;
        }

        public static IActionResult AddHeader([NotNull] this IActionResult actionResult, [NotNull] string key, string value)
        {
            return AddHeader(actionResult, key, value, Always);
        }

        public static IActionResult AddHeader([NotNull] this IActionResult actionResult,
                                              [NotNull] string key,
                                              [NotNull] string value,
                                              [NotNull] Predicate<ActionResultContext> predicate)
        {
            return new ActionResultWrapper(actionResult, (context) =>
            {
                if (predicate(context))
                {
                    AddHeader(context, key, value);
                }
            });
        }
    }
}
