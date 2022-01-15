
namespace Books.Interfaces
{
    public interface IUnitOfWork<T> where T : class  
    {
        IGenericRepository<T> Entity { get; }
        Task<bool> SaveAsync();
        Task CompleteAsync();
    }
}
