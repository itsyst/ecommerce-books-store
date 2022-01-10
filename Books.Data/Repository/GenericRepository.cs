using Books.Interfaces;
using System.Linq.Expressions;
using Books.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Books.Data.Repository
{
#pragma warning disable CS8603

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _table;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
            _table = _context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return _table.AsEnumerable().ToList();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = _table;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public T GetById(object id)
        {
            return _table.Find(id);
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true)
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
                return query.FirstOrDefault();
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
                return query.FirstOrDefault();
            }
        }

        public void Insert(T entity)
        {
            _table.Add(entity);
        }

        public void Delete(object id)
        {
            T existing = GetById(id);
            _table.Remove(existing);
        }

        public void DeleteRange(IEnumerable<T> entity)
        {
            _table.RemoveRange(entity);
        }

        public void Update(T entity)
        {
            _table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public bool Exists(object id)
        {
            return _context.Find((Type)id) == null ? false : true;
        }
    }
}
