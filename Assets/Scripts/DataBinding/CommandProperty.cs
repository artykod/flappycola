using System;

namespace DataBinding
{
    public class CommandProperty : ICommandProperty
    {
        private Action<IDataSource> _callback;

        public string Name { get; }

        public CommandProperty(string name, Action<IDataSource> callback)
        {
            Name = name;
            _callback = callback;
        }

        public void Dispose()
        {
            _callback = null;
        }

        public void Subscribe(IDataObservator subscriber) {}
        public void Unsubscribe(IDataObservator subscriber) {}

        public void Execute(IDataSource dataSource)
        {
            _callback?.Invoke(dataSource);
        }
    }
}