using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.AspNet.Razor.TagHelpers;

namespace ActivatorWebSite.TagHelpers
{
    [TagName("*")]
    [ContentBehavior(ContentBehavior.Modify)]
    public class HiddenTagHelper : TagHelper
    {
        public bool HideContent { get; set; }

        public string Name { get; set; }

        [Activate]
        public IHtmlHelper HtmlHelper { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!HideContent)
            {
                return;
            }
            output.Content = HtmlHelper.Hidden(Name, output.Content).ToString();
        }
    }
}