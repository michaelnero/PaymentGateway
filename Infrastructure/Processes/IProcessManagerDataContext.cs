using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Processes
{
    public interface IProcessManagerDataContext<T> : IDisposable where T : class, IProcessManager
    {
        Task<T> FindAsync(Guid id);

        Task SaveAsync(T processManager);

        Task<T> FindAsync(Expression<Func<T, bool>> predicate);

        Task<T> FindAsync(Expression<Func<T, bool>> predicate, bool includeCompleted);
    }
}
