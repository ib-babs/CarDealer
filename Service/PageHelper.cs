using CarDealer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CarDealer.Service
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageHelper(IUrlHelperFactory urlHelperFactory) : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }
        public string? PageAction { get; set; }
        public PageItem? PageModel { get; set; }
        public bool IsPageSelected { get; set; }
        public string PageSelectedClass { get; set; } = String.Empty;
        public string PageClass { get; set; } = String.Empty;
        public string PageNormalClass { get; set; } = String.Empty;


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //Console.WriteLine($"\n\n{PageModel.TotalPage}\n\n");
            if (ViewContext != null && PageModel != null)
            {
                IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                TagBuilder div = new("div");
                for (int i = 1; i <= PageModel.TotalPage; i++)
                {
                    var url = urlHelper.Action(PageAction, new { productPage = i });
                    TagBuilder anchorTag = new("a");
                    anchorTag.Attributes.Add("href", url);
                    if (IsPageSelected)
                    {
                        anchorTag.AddCssClass(PageClass);
                        anchorTag.AddCssClass(i == PageModel.CurrentPage ? PageSelectedClass : PageNormalClass);
                    }
                    anchorTag.InnerHtml.Append(i.ToString());
                    div.InnerHtml.AppendHtml(anchorTag);
                }
                output.Content.AppendHtml(div.InnerHtml);
            }
        }




    }
}
