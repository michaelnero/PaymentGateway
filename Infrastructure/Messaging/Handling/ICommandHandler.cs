using System.Threading.Tasks;

namespace Infrastructure.Messaging.Handling
{
    public interface ICommandHandler
    {

    }
    
    public interface ICommandHandler<T> : ICommandHandler where T : ICommand
	{
		Task HandleAsync(T command);
	}
}
