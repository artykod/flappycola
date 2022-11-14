using System;

namespace DataBinding
{
    public interface IDataProperty : IDataNode
    {
        Type NativeType { get; }
        ValueT GetValue<ValueT>();
        void SetValue<ValueT>(ValueT value);
    }

    public class DataProperty<T> : IDataProperty
    {
        private readonly string _name;

        private DataObservatorsContainer _subscriberContainer;

        private T _data;

        public string Name => _name;

        public DataProperty(string name, T data)
        {
            _name = name;

            SetValue(data);
        }

        public DataProperty(string name) : this(name, default)
        { }

        public Type NativeType => typeof(T);

        public ValueT GetValue<ValueT>()
        {
            return DataConverter.Convert<T, ValueT>(_data);
        }

        public void SetValue<ValueT>(ValueT value)
        {
            var newData = DataConverter.Convert<ValueT, T>(value);

            if (!object.Equals(_data, newData))
            {
                _data = newData;

                _subscriberContainer?.HandleChanges();
            }
        }

        public void Subscribe(IDataObservator subscriber)
        {
            _subscriberContainer = _subscriberContainer ?? new DataObservatorsContainer(this);

            _subscriberContainer.Subscribe(subscriber);
        }

        public void Unsubscribe(IDataObservator subscriber)
        {
            _subscriberContainer?.Unsubscribe(subscriber);
        }

        public IDataNode Clone(string name)
        {
            return new DataProperty<T>(name, _data);
        }
    }
}