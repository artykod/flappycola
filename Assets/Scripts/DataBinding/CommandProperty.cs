using System;

namespace DataBinding
{
    public class CommandProperty : ICommandProperty
    {
        private Action<IDataSource> _action;

        public string Name { get; }

        public CommandProperty(string name, Action<IDataSource> action)
        {
            Name = name;

            SetAction(action);
        }

        public CommandProperty(string name) : this(name, default)
        {}

        public void Dispose()
        {
            _action = null;
        }

        public void Execute(IDataSource dataSource)
        {
            _action?.Invoke(dataSource);
        }

        public void SetAction(Action<IDataSource> action)
        {
            _action = action;
        }

        public void Subscribe(IDataObservator subscriber) {}
        public void Unsubscribe(IDataObservator subscriber) {}

        public IDataNode Clone(string name)
        {
            return new CommandProperty(name, _action);
        }
    }
}