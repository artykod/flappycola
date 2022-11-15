using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    public class EnableBinding : BaseDataBinding
    {
        [SerializeField] private string _path;
        [SerializeField] private bool _invert;

        protected override bool HandleChangesWhenInactive => true;

        protected override void GatherPathToSubscribe(List<string> result)
        {
            result.Add(_path);
        }

        protected override void BindData(IDataSource dataSource)
        {
            var value = dataSource != null && dataSource.TryGetNode<IDataProperty>(_path, out var valueProperty) 
                && valueProperty.GetValue<bool>();

            gameObject.SetActive(value ^ _invert);
        }
    }
}