using Microsoft.EntityFrameworkCore;
using Stories.Model;

namespace Stories.Data
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options)
            : base(options)
        {
        }

        public DbSet<Log> Log { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder1)
        {
            modelBuilder1.Entity<Log>().ToTable("Logs");
        }
    }
}