using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stories.Data;
using Stories.Model;

namespace Stories.Pages.My.Friends
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _dbContext { get; set; }
        public User ActualUser { get; set; }

        public IndexModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnGetAsync()
        {
            ActualUser = await _dbContext.Users.Where(s => s.Email == User.Identity.Name).FirstOrDefaultAsync();
        }
    }
}
