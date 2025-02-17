using System.Linq.Expressions;

namespace Domain.Infrastructure
{
    public interface IRepository
    {
        Task<ICollection<T>> GetAll<T>() where T : class;
        Task<ICollection<T>> Get<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<T?> GetSingleOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task Add<T>(T entityToAdd) where T : class;
        Task Save();
    }
}
