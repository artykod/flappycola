using System;
using UnityEngine;

namespace DataBinding
{
    [Serializable]
    public class DataSourceFilterPredicateEntry : ISerializationCallbackReceiver
    {
        [SerializeField] private string _data;

        public BaseDataFilterCondition Condition { get; private set; }

        public DataSourceFilterPredicateEntry(BaseDataFilterCondition condition)
        {
            Condition = condition;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _data = Condition != null ? DataSourceFilterSerializator.SerializeCondition(Condition) : null;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Condition = !string.IsNullOrEmpty(_data) ? DataSourceFilterSerializator.DeserializeCondition(_data) : null;
        }
    }
}