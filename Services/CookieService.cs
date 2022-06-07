using Microsoft.AspNetCore.Http;
using Stories.Data;
using Stories.Tools;
using System;
using Microsoft.EntityFrameworkCore;
using Stories.Model;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Stories.Services
{

    public class CookieService :  ICookieService 
    {
        private readonly IServiceProvider _serviceProvider;
        private IHttpContextAccessor _httpContextAccessor;
       

        public CookieService(IHttpContextAccessor httpContextAccessor,IServiceProvider serviceProvider)
        {
            
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
         
        }

        public bool SetCookie(string key, string value)
        {
            Console.WriteLine("SettingCookie");
            CookieOptions options = new CookieOptions();
            options.Domain = "stories.afrowave.ltd";
            options.Expires = DateTime.Now.AddMonths(12);
            options.Secure = true;
            try
            {
                Console.WriteLine(key + " " + value);
                _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return true;
        }

        public string GetCookie(string key)
        {

            if (_httpContextAccessor.HttpContext.Request.Cookies[key] != null)
                return _httpContextAccessor.HttpContext.Request.Cookies[key];
            return "";
        }

        public string GetCookie(string key, string defaultValue)
        {
            Console.WriteLine("We are in GetCookie with two arguments");
            if (_httpContextAccessor.HttpContext.Request.Cookies[key] != null)
            {
                if (_httpContextAccessor.HttpContext.Request.Cookies[key] != null)
                {
                    Console.WriteLine("Cookie Exists");
                    return _httpContextAccessor.HttpContext.Request.Cookies[key];
                }
            }
            Console.WriteLine("Cookie Doesn't Exist, creating it");
            if (SetCookie(key, defaultValue))
                return _httpContextAccessor.HttpContext.Request.Cookies[key];
            return "";
        }

        public async Task<string> GetTheme()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the DbContext instance
                var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (GetCookie("Theme") is null)
                {
                    SetCookie("Theme", Settings.DefaultTheme);
                    return await Task.FromResult(Settings.DefaultTheme);
                }
                List<Theme> availableThemes = await _dbContext.Themes.ToListAsync();

                if (!availableThemes.Where(s => s.Name == GetCookie("Theme")).Any())
                    return await SetTheme(Settings.DefaultTheme);

                return GetCookie("Theme");
            }
        }
        public async Task<string> GetThemeAsync(string userName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the DbContext instance
                var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                User actualUser = await _dbContext.Users.Where(s => s.Email == userName).Include(s => s.Theme).FirstOrDefaultAsync();
                if (actualUser == null) 
                    return await GetTheme();
                if (actualUser.ThemeId == null) return await SetTheme(await GetTheme(), userName);

                return actualUser.Theme.Name;
            }
        }

        public async Task<string> ToggleTheme()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the DbContext instance
                var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                Theme newTheme = null; 
                List<Theme> availableThemes =  await _dbContext.Themes.ToListAsync();
                Console.WriteLine("Found" + availableThemes.Count() + " themes");
                if (GetCookie("Theme") is null)
                {
                    SetCookie("Theme", Settings.DefaultTheme);
                    return (Settings.DefaultTheme);
                }
                string actualCookie = GetCookie("Theme");
                if (await _dbContext.Themes.Where(s => s.Name == actualCookie).AnyAsync())
                {   
                    var actualTheme = availableThemes.Where(s => s.Name ==  actualCookie).FirstOrDefault();
                    var index = availableThemes.IndexOf(actualTheme);
                    if ((index + 1) > (availableThemes.Count() - 1))
                        newTheme = availableThemes[0];
                    else
                        newTheme = availableThemes[index + 1];

                    return await SetTheme(newTheme.Name);
                    
                }
                return await SetTheme(Settings.DefaultTheme);
                
            }
        }

        public async Task<string> ToggleTheme(string userName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                List<Theme> availableThemes = await _dbContext.Themes.ToListAsync();
                
                string actualThemeName = "";
                Theme actualTheme = null;
                string newThemeName = "";
                Theme newTheme = null;
                User actualUser = await _dbContext.Users.Where(s => s.Email == userName).Include(s => s.Theme).FirstOrDefaultAsync();
                

                if (actualUser is null)
                    return await ToggleTheme();

                if (actualUser.ThemeId == null)
                {
                    actualThemeName = await GetTheme();
                    
                }
                else
                {
                    actualThemeName = actualUser.Theme.Name;
                }
                actualTheme = availableThemes.Where(s => s.Name == actualThemeName).FirstOrDefault();
                int themeIndex = availableThemes.IndexOf(actualTheme);
               
                if ((themeIndex + 1) > (availableThemes.Count()-1))
                    newTheme = availableThemes[0];
                else
                    newTheme = availableThemes[themeIndex + 1];

               

                return await SetTheme(newTheme.Name, userName);

            }
        }

        public async Task<string> SetTheme(string theme)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the DbContext instance
                var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                SetCookie("Theme", theme);
                return await Task.FromResult(theme);
                
            }
        }

        public async Task<string> SetTheme(string theme, string userName)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Get the DbContext instance
                var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                User actualUser = await _dbContext.Users.Where(s => s.Email == userName).FirstOrDefaultAsync();
                int newThemeId = await _dbContext.Themes.Where(s => s.Name == theme).Select(s => s.Id).FirstOrDefaultAsync();
                actualUser.ThemeId = newThemeId;
                await _dbContext.SaveChangesAsync();
                return await SetTheme(theme);
            }
        }
    }
}