using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DataBinding
{
    public class ColorBinding : BaseDataBinding
    {
        [SerializeField] private string _path;

        protected override void GatherPathToSubscribe(List<string> result)
        {
            result.Add(_path);
        }

        protected override void BindData(IDataSource dataSource)
        {
            if (dataSource != null &&  dataSource.TryGetNodeByPath<IDataProperty>(_path, out var valueProperty)
                && ColorUtility.TryParseHtmlString(valueProperty.GetValue<string>(), out var color))
            {
                GetComponent<Graphic>().color = color;
            }
        }
    }
}