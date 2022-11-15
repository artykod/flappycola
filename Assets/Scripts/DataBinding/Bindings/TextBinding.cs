using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    public class TextBinding : BaseTextBinding
    {
        [SerializeField] private string _path;

        protected override void GatherPathToSubscribe(List<string> result)
        {
            result.Add(_path);
        }

        protected override void BindData(IDataSource dataSource)
        {
            if (dataSource != null && dataSource.TryGetNodeByPath<IDataProperty>(_path, out var valueProperty))
            {
                ApplyText(ProcessText(valueProperty.GetValue<string>()));
            }
        }
    }
}