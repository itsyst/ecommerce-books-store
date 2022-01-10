using System.Linq.Expressions;

namespace Books.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        T GetById(object id);
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true);
        void Insert(T entity);
        void Update(T entity);
        void Delete(object id);
        void DeleteRange(IEnumerable<T> entity);
        bool Exists(object id);

    }
}
