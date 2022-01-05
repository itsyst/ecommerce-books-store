using BooksWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksWebApp.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

    }
}
