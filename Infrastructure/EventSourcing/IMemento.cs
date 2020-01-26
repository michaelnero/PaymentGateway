namespace Infrastructure.EventSourcing
{
    public interface IMemento
    {
        int Version { get; }
    }
}
