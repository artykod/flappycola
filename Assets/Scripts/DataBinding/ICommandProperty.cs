namespace DataBinding
{
    public interface ICommandProperty: IDataNode
    {
        void Execute(IDataSource dataSource);
    }
}