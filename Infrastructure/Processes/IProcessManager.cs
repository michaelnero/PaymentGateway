using System;
using System.Collections.Generic;
using Infrastructure.Messaging;

namespace Infrastructure.Processes
{
    public interface IProcessManager
    {
        Guid Id { get; }

        bool Completed { get; }

        IEnumerable<Envelope<ICommand>> Commands { get; }
    }
}
