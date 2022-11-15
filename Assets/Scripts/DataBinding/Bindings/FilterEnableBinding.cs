using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    public class FilterEnableBinding : BaseDataBinding
    {
        [SerializeField] private DataSourceFilter _filter;

        protected override bool HandleChangesWhenInactive => true;

        protected override void GatherPathToSubscribe(List<string> result)
        {
            _filter.AppendPathToSubscription(result);
        }

        protected override void BindData(IDataSource dataSource)
        {
            gameObject.SetActive(dataSource != null && _filter.IsMatch(dataSource));
        }
    }
}