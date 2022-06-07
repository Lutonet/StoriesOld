using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Stories.Pages.Messages
{
    public static class ManageNavPages
    {
        public static string Index => "Index";
        public static string Inbox => "Inbox";
        public static string Compose => "Compose";
        public static string Deleted => "Deleted";
        public static string Sent => "Sent";
        public static string Settings => "Settings";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string InboxNavClass(ViewContext viewContext) => PageNavClass(viewContext, Inbox);

        public static string ComposeNavClass(ViewContext viewContext) => PageNavClass(viewContext, Compose);

        public static string DeletedNavClass(ViewContext viewContext) => PageNavClass(viewContext, Deleted);

        public static string SentNavClass(ViewContext viewContext) => PageNavClass(viewContext, Sent);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}