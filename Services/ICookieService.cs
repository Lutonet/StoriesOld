using System.Threading.Tasks;

namespace Stories.Services
{
    public interface ICookieService
    {
        bool SetCookie(string key, string value);

        string GetCookie(string key);

        string GetCookie(string key, string defaultValue);
        Task<string> GetTheme();
        Task<string> GetThemeAsync (string userName);
        Task<string> SetTheme(string theme);
        Task<string> SetTheme(string theme, string userName);
        Task<string> ToggleTheme();
        Task<string> ToggleTheme(string userName);
    }
}