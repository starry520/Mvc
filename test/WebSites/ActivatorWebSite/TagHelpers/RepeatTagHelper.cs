using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.AspNet.Razor.TagHelpers;

namespace ActivatorWebSite.TagHelpers
{
    [TagName("*")]
    [ContentBehavior(ContentBehavior.Modify)]
    public class RepeattTagHelper : TagHelper
    {
        public int Repeat { get; set; }

        public string Expression { get; set; }

        [Activate]
        public IHtmlHelper HtmlHelper { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var repeatContent = output.Content;
            if (!string.IsNullOrEmpty(Expression))
            {
                repeatContent = (string)HtmlHelper.ViewData.Eval(Expression);
            }

            for (int i = 0; i < Repeat; i++)
            {
                output.Content += repeatContent;
            }
        }

    }
}