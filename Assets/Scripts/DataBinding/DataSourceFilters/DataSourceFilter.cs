using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    [Serializable]
    public class DataSourceFilter
    {
        [SerializeField] private List<DataSourceFilterPredicate> _predicates;

        public List<DataSourceFilterPredicate> Predicates =>
            _predicates ?? (_predicates = new List<DataSourceFilterPredicate>());

        public bool IsMatch(IDataSource dataSource)
        {
            if (_predicates == null || _predicates.Count == 0)
            {
                return true;
            }

            foreach (var condition in _predicates)
            {
                if (condition.IsMatch(dataSource))
                {
                    return true;
                }
            }

            return false;
        }

        public void AppendPathToSubscription(List<string> result)
        {
            foreach (var predicate in _predicates)
            {
                predicate.AppendPathToSubscription(result);
            }
        }
    }
}