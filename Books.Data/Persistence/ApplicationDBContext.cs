using Books.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Books.Data.Persistence
{
#pragma warning disable CS8618
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

    }
}
