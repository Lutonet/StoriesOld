using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Stories.Data;
using Stories.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace Stories.Areas.Identity.Pages.Account.Manage
{
    public class ProfilePictureModel : PageModel
    {
        [TempData]
        public string StatusMessage { get; set; }

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<InfoModel> _logger;
        private readonly IWebHostEnvironment _env;
        public string UserName { get; set; }
        public string RelativePath { get; set; }
        public string ImagePath { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public ProfilePictureModel(
            UserManager<User> userManager,
            ILogger<InfoModel> logger,
            ApplicationDbContext dbContext,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _logger = logger;
            _dbContext = dbContext;
            _env = env;
        }

        public class InputModel
        {
            [Display(Name = "Tell us something about you: ")]
            public IFormFile ProfileImage { get; set; }

            public string FileName { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Input = new InputModel();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            string picturePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", user.Id, "ProfilePhoto_180.png");
            Console.WriteLine(picturePath);
            if (!System.IO.File.Exists(picturePath)) ImagePath = "/img/admin.png";
            else ImagePath = "/Images/" + user.Id + "/ProfilePhoto_180.png";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string filename, IFormFile blob)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            // if user pictures folder doesn't exist create it
            string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", user.Id);
            RelativePath = System.IO.Path.GetRelativePath(_env.WebRootPath, Directory.GetCurrentDirectory());


            try
            {
                using (var image = Image.Load(blob.OpenReadStream()))
                {
                    string systemFileExtenstion = filename.Substring(filename.LastIndexOf('.'));
                    if (!Directory.Exists(root))
                    {
                        try
                        {
                            Directory.CreateDirectory(root);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Can't create user directory:" + ex.Message, ex);
                            Console.WriteLine("Can't create user directory ID: " + user.Id);
                            StatusMessage = "Error: Error creating User directory";
                            return RedirectToPage();
                        }
                    }


                    image.Mutate(x => x.Resize(180, 180));
                    var newfileName180 = "ProfilePhoto_180" + systemFileExtenstion;
                    var filepath160 = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", user.Id)).Root + $@"\{newfileName180}";
                    image.Save(filepath160);

                    var newfileName100 = "ProfilePhoto_100" + systemFileExtenstion;
                    var filepath100 = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", user.Id)).Root + $@"\{newfileName100}";
                    image.Mutate(x => x.Resize(100, 100));
                    image.Save(filepath100);

                    var newfileName32 = "ProfilePhoto_32" + systemFileExtenstion;
                    var filepath32 = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", user.Id)).Root + $@"\{newfileName32}";
                    image.Mutate(x => x.Resize(32, 32));
                    image.Save(filepath32);
                }
                StatusMessage = "Update OK";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError("Can't save a picture", ex);
                StatusMessage = "Error: Update Error";

                return RedirectToPage();
            }
        }
    }
}