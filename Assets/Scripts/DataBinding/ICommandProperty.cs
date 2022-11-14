using System;

namespace DataBinding
{
    public interface ICommandProperty : IDataNode
    {
        void Execute(IDataSource dataSource);
        void SetAction(Action<IDataSource> action);
    }
}