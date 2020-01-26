using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public static class CommandBusExtensions
    {
        public static Task SendAsync(this ICommandBus bus, ICommand command)
        {
            if (bus is null) throw new ArgumentNullException(nameof(bus));
            if (command is null) throw new ArgumentNullException(nameof(command));
            
            return bus.SendAsync(new Envelope<ICommand>(command));
        }

        public static Task SendAsync(this ICommandBus bus, IEnumerable<ICommand> commands)
        {
            if (bus is null) throw new ArgumentNullException(nameof(bus));
            if (commands is null) throw new ArgumentNullException(nameof(commands));
            
            return bus.SendAsync(commands.Select(x => new Envelope<ICommand>(x)));
        }
    }
}
