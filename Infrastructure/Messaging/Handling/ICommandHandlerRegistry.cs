namespace Infrastructure.Messaging.Handling
{
    public interface ICommandHandlerRegistry
	{
		void Register(ICommandHandler handler);
	}
}
