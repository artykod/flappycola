using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    public class TextBinding : BaseTextBinding
    {
        [SerializeField] private string _path;

        protected override void FillPathToSubscription(List<string> result)
        {
            result.Add(_path);
        }

        protected override void BindDataInternal(IDataNode property)
        {
            if (DataSource.TryGetNodeByPath<IDataProperty>(_path, out var valueProperty))
            {
                ApplyText(ProcessText(valueProperty.GetValue<string>()));
            }
        }
    }
}