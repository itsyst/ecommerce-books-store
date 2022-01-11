using Books.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Books.Data.Persistence
{
#pragma warning disable CS8618
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Cover> Cover { get; set; }


    }
}
