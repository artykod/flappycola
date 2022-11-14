using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DataBinding
{
    public class ColorBinding : BaseDataBinding
    {
        [SerializeField] private string _path;

        protected override void FillPathToSubscription(List<string> result)
        {
            result.Add(_path);
        }

        protected override void BindDataInternal(IDataNode property)
        {
            if (DataSource.TryGetNodeByPath<IDataProperty>(_path, out var valueProperty)
                && ColorUtility.TryParseHtmlString(valueProperty.GetValue<string>(), out var color))
            {
                GetComponent<Graphic>().color = color;
            }
        }
    }
}