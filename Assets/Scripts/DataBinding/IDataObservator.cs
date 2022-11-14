namespace DataBinding
{
    public interface IDataObservator
    {
        void HandleChanges(IDataNode property);
    }
}