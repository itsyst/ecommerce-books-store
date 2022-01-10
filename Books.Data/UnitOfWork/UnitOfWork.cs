using Books.Data.Persistence;
using Books.Data.Repository;
using Books.Interfaces;

namespace Books.Data.UnitOfWork
{

# nullable disable
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private IGenericRepository<T> _entity;
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

        public void Save()
        {
            _context.SaveChanges();
        }
  
        Task IUnitOfWork<T>.SaveAsync()
        {
            return _context.SaveChangesAsync();  
        }
    }
}
