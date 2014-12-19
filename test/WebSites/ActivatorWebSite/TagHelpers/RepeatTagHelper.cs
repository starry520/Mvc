using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.AspNet.Razor.TagHelpers;

namespace ActivatorWebSite.TagHelpers
{
    [TagName("div")]
    [ContentBehavior(ContentBehavior.Modify)]
    public class RepeatTagHelper : TagHelper
    {
        public int Repeat { get; set; }

        public ModelExpression Expression { get; set; }

        [Activate]
        public IHtmlHelper HtmlHelper { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var repeatContent = HtmlHelper.Encode(Expression.Metadata.Model.ToString());

            if (string.IsNullOrEmpty(repeatContent))
            {
                repeatContent = output.Content;
                output.Content = string.Empty;
            }

            for (int i = 0; i < Repeat; i++)
            {
                output.Content += repeatContent;
            }
        }

    }
}