namespace Infrastructure.EventSourcing
{
    public interface IMementoOriginator
    {
        IMemento SaveToMemento();
    }
}
