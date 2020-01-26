using System;
using System.Threading.Tasks;

namespace Infrastructure.EventSourcing
{
    public interface IEventSourcedRepository<T> where T : IEventSourced
    {
        Task<T> FindAsync(Guid id);

        Task<T> GetAsync(Guid id);

        Task SaveAsync(T eventSourced, string correlationId);
    }
}
