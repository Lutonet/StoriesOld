using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Stories.Data;
using Stories.Hubs;
using Stories.Model;
using Stories.Services;
using System;

namespace Stories
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine(Environment.CurrentDirectory);
            services.AddDbContext<LogDbContext>(options =>
               options.UseSqlite("Data Source=log.db"));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(

                    Configuration.GetConnectionString("TestConnection"),
                     o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)

                    ));
            services.AddControllersWithViews(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });
            services.AddHttpContextAccessor();
            services.AddSingleton<ISmsSender, SmsSender>();
            services.AddTransient<ICookieService, CookieService>();
            services.Configure<AppSettings>(Configuration.GetSection("SmtpSettings"));
            services.AddScoped<IEmailService, EmailService>();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Stories", Version = "v1" });
            });
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration.GetValue("FacebookAppId", "0");
                facebookOptions.AppSecret = Configuration.GetValue("FacebookAppSecret", "0");
                facebookOptions.AccessDeniedPath = "/Identity/Account/AccessDenied";
            }).AddTwitter(twitterOptions =>
            {
                twitterOptions.ConsumerKey = Configuration.GetValue("TwitterApiKey", "0");
                twitterOptions.ConsumerSecret = Configuration.GetValue("TwitterAppSecret", "0");
                twitterOptions.RetrieveUserDetails = true;
            }).AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = Configuration.GetValue("MicrosoftClientId", "0");
                microsoftOptions.ClientSecret = Configuration.GetValue("MicrosoftAppSecret", "0");
            }).AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration.GetValue("GoogleClientId", "0");
                googleOptions.ClientSecret = Configuration.GetValue("GoogleClientSecret", "0");
            });

            services.AddMvc();
            services.AddRazorPages();
            services.AddSignalR();
            services.AddHostedService<MigratorHostedService>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                // User settiongs
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890-.@+";
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(365);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapHub<IndexHub>("/indexHub");
                endpoints.MapHub<MessageHub>("/MessageHub");
                endpoints.MapHub<CollectionHub>("/CollectionHub");
            });
        }
    }
}