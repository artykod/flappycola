using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    public class EnableBinding : BaseDataBinding
    {
        [SerializeField] private string _path;
        [SerializeField] private bool _invert;

        protected override bool HandleChangesWhenInactive => true;

        protected override void FillPathToSubscription(List<string> result)
        {
            result.Add(_path);
        }

        protected override void BindDataInternal(IDataNode property)
        {
            var value = DataSource.TryGetNode<IDataProperty>(_path, out var valueProperty) && valueProperty.GetValue<bool>();

            gameObject.SetActive(value ^ _invert);
        }
    }
}