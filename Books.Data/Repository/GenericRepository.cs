using Books.Data.Persistence;
using Books.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Books.Data.Repository
{
#pragma warning disable CS8603

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal readonly DbSet<T> _table;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _table = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _table;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties)
                {
                    query = query.Include(includeProp);
                }

            }

            return await query.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await _table.FindAsync(id);
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true)
        {

            if (tracked)
            {
                IQueryable<T> query = _table;

                query = query.Where(filter);
                if (includeProperties != null)
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }

                return await query.FirstOrDefaultAsync();
            }
            else
            {
                IQueryable<T> query = _table.AsNoTracking();

                query = query.Where(filter);
                if (includeProperties != null)
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }

                return await query.FirstOrDefaultAsync();
            }

        }

        public virtual async Task<bool> InsertAsync(T entity)
        {
            await _table.AddAsync(entity);

            return true;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            return await Task.FromResult(true);

        }

        public async Task<bool> DeleteAsync(object id)
        {
            T existing = await GetByIdAsync(id);
            if (existing == null) return false;
            _table.Remove(existing);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<T> entity)
        {
            _table.RemoveRange(entity);
            return await Task.FromResult(true);
        }

        public async Task<bool> ExistsAsync(object id)
        {
            return await _context.FindAsync((Type)id) != null;
        }
    }
}
