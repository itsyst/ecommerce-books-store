using Books.Data.Persistence;
using Books.Data.Repository;
using Books.Interfaces;

namespace Books.Data.UnitOfWork
{

# nullable disable
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public IGenericRepository<T> _entity;
        private bool _disposed;
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


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
