using Books.Data.Persistence;
using Books.Data.Repository;
using Books.Data.UnitOfWork;
using Books.Domain.Entities;
using Books.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Books.Data
{
    public static class PersistenceContainer
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                    );

            services.AddScoped(typeof(IGenericRepository<ApplicationUser>), typeof(GenericRepository<ApplicationUser>));
            services.AddScoped(typeof(IGenericRepository<Category>), typeof(GenericRepository<Category>));
            services.AddScoped(typeof(IGenericRepository<Company>), typeof(GenericRepository<Company>));
            services.AddScoped(typeof(IGenericRepository<Cover>), typeof(GenericRepository<Cover>));
            services.AddScoped(typeof(IGenericRepository<OrderDetail>), typeof(GenericRepository<OrderDetail>));
            services.AddScoped(typeof(IGenericRepository<OrderHeader>), typeof(GenericRepository<OrderHeader>));
            services.AddScoped(typeof(IGenericRepository<Product>), typeof(GenericRepository<Product>));
            services.AddScoped(typeof(IGenericRepository<ShoppingCart>), typeof(GenericRepository<ShoppingCart>));

            services.AddScoped(typeof(IUnitOfWork<ApplicationUser>), typeof(UnitOfWork<ApplicationUser>));
            services.AddScoped(typeof(IUnitOfWork<Author>), typeof(UnitOfWork<Author>));
            services.AddScoped(typeof(IUnitOfWork<Category>), typeof(UnitOfWork<Category>));
            services.AddScoped(typeof(IUnitOfWork<Company>), typeof(UnitOfWork<Company>));
            services.AddScoped(typeof(IUnitOfWork<Cover>), typeof(UnitOfWork<Cover>));
            services.AddScoped(typeof(IUnitOfWork<OrderDetail>), typeof(UnitOfWork<OrderDetail>));
            services.AddScoped(typeof(IUnitOfWork<OrderHeader>), typeof(UnitOfWork<OrderHeader>));
            services.AddScoped(typeof(IUnitOfWork<Product>), typeof(UnitOfWork<Product>));
            services.AddScoped(typeof(IUnitOfWork<ShoppingCart>), typeof(UnitOfWork<ShoppingCart>));

            services.AddScoped(typeof(IDbInitializer), typeof(DbInitializer));

            return services;
        }
    }
}
