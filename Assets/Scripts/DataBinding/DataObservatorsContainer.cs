using System;
using System.Collections.Generic;
using System.Linq;

namespace DataBinding
{
    public class DataObservatorsContainer : IDisposable
    {
        private readonly IDataNode _property;
        private HashSet<IDataObservator> _subscribers;

        public DataObservatorsContainer(IDataNode property)
        {
            _property = property;
        }

        public void Dispose()
        {
            _subscribers?.Clear();
        }

        public void Subscribe(IDataObservator subscriber)
        {
            _subscribers = _subscribers ?? new HashSet<IDataObservator>();

            _subscribers.Add(subscriber);
        }

        public void Unsubscribe(IDataObservator subscriber)
        {
            _subscribers?.Remove(subscriber);
        }

        public void HandleChanges()
        {
            if (_subscribers == null || _subscribers.Count == 0)
            {
                return;
            }

            foreach (var subscriber in _subscribers.ToList())
            {
    #if UNITY_EDITOR
                if (subscriber is UnityEngine.Object uObject && uObject == null)
                {
                    var subscriberType = subscriber.GetType();
                    var propertyName = _property.Name;

                    UnityEngine.Debug.LogError($"LEAK '{subscriberType}' in data property '{propertyName}'");
                }
    #endif
                subscriber.HandleChanges(_property);
            }
        }
    }
}