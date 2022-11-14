namespace DataBinding
{
    public interface IDataNode
    {
        string Name { get; }

        void Subscribe(IDataObservator subscriber);
        void Unsubscribe(IDataObservator subscriber);
    }
}