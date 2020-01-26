using System;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    public interface IDataContext<T> : IDisposable where T : IAggregateRoot
    {
        Task<T> FindAsync(Guid id);

        Task SaveAsync(T aggregate);
    }
}
