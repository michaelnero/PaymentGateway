using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public interface ICommandBus
    {
        Task SendAsync(Envelope<ICommand> command);

        Task SendAsync(IEnumerable<Envelope<ICommand>> commands);
    }
}
