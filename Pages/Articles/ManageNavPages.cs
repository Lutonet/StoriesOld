using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Stories.Pages.Articles
{
    public static class ManageNavPages
    {
        public static string Index => "Index";
        public static string Categories => "Categories";
        public static string Groups => "Groups";

        public static string ArticlesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string CategoriesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Categories);

        public static string GroupsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Groups);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}