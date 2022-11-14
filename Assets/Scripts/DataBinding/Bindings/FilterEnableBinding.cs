using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    public class FilterEnableBinding : BaseDataBinding
    {
        [SerializeField] private DataSourceFilter _filter;

        protected override bool HandleChangesWhenInactive => true;

        protected override void FillPathToSubscription(List<string> result)
        {
            _filter.AppendPathToSubscription(result);
        }

        protected override void BindDataInternal(IDataNode property)
        {
            gameObject.SetActive(_filter.IsMatch(DataSource));
        }
    }
}