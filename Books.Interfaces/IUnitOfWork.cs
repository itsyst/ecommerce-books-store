
namespace Books.Interfaces
{
    public interface IUnitOfWork<T> : IDisposable where T : class  
    {
        IGenericRepository<T> Entity { get; }
        Task<bool> SaveAsync();
        Task CompleteAsync();
    }
}
