using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Stories.Pages.Administration.Admin
{
    public static class ManageNavPages
    {
        public static string Index => "Index";

        public static string Users => "Users";

        public static string Logs => "Logs";

        public static string Reports => "Reports";

        public static string EmailLogs => "EmailLogs";

        public static string UserLogs => "UserLogs";

        public static string SystemSettings => "SystemSettings";

        public static string ScheduledTasks => "Tasks";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);

        public static string LogsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Logs);

        public static string ReportsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Reports);

        public static string EmailLogsNavClass(ViewContext viewContext) => PageNavClass(viewContext, EmailLogs);

        public static string UserLogsNavClass(ViewContext viewContext) => PageNavClass(viewContext, UserLogs);

        public static string SystemSettingsNavClass(ViewContext viewContext) => PageNavClass(viewContext, SystemSettings);

        public static string ScheduledTasksNavClass(ViewContext viewContext) => PageNavClass(viewContext, ScheduledTasks);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}