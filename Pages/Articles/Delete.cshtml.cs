using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stories.Model;
using System.Threading.Tasks;

namespace Stories.Pages.Articles
{
    public class DeleteModel : PageModel
    {
        private readonly Stories.Data.ApplicationDbContext _context;

        public DeleteModel(Stories.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Article Article { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Article = await _context.Articles
                .Include(a => a.AgeRestriction)
                .Include(a => a.RecommendedAgeGroup)
                .Include(a => a.User).FirstOrDefaultAsync(m => m.Id == id);

            if (Article == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Article = await _context.Articles.FindAsync(id);

            if (Article != null)
            {
                _context.Articles.Remove(Article);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}