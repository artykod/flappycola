using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    [Serializable]
    public class DataSourceFilterPredicate
    {
        [SerializeField] private List<DataSourceFilterPredicateEntry> _entries;

        public List<DataSourceFilterPredicateEntry> Entries =>
            _entries ?? (_entries = new List<DataSourceFilterPredicateEntry>());

        public bool IsMatch(IDataSource dataSource)
        {
            if (_entries == null || _entries.Count == 0)
            {
                return true;
            }

            foreach (var condition in _entries)
            {
                if (condition.Condition != null && !condition.Condition.IsMatch(dataSource))
                {
                    return false;
                }
            }

            return true;
        }

        public void AppendPathToSubscription(List<string> result)
        {
            if (_entries == null || _entries.Count == 0)
            {
                return;
            }

            foreach (var entry in _entries)
            {
                if (entry.Condition != null)
                {
                    result.Add(entry.Condition.Path);
                }
            }
        }
    }
}