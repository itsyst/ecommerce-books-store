using Books.Data.Persistence;
using Books.Data.Repository;
using Books.Interfaces;
using Microsoft.Extensions.Logging;

namespace Books.Data.UnitOfWork
{

# nullable disable
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class 
    {
        private readonly ApplicationDbContext _context;
        public IGenericRepository<T> _entity;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

    public IGenericRepository<T> Entity
        {

            get
            {
               return _entity ??= new GenericRepository<T>(_context);
            }

        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
 
        public async Task<bool> SaveAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
