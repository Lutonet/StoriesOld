using Microsoft.AspNetCore.Mvc;
using Stories.Services;

namespace Stories.API
{
    [Route("api/togletheme")]
    [ApiController]
    public class ThemeSelector : ControllerBase
    {
        private ICookieService _cookieService;

        public ThemeSelector(ICookieService cookieService)
        {
            _cookieService = cookieService;
        }

        [HttpGet]
        public IActionResult OnGet(string? returnUrl)
        {
            if (Request.Cookies["Theme"] == "site-light.css")
            {
                _cookieService.SetCookie("Theme", "site-dark.css");
            }
            else
            {
                _cookieService.SetCookie("Theme", "site-light.css");
            }
            return LocalRedirect("/Index");
        }
    }
}