using System;

namespace Infrastructure.Database
{
    public interface IAggregateRoot
    {
        Guid Id { get; }
    }
}
