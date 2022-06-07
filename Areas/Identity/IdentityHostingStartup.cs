using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Stories.Areas.Identity.IdentityHostingStartup))]

namespace Stories.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}