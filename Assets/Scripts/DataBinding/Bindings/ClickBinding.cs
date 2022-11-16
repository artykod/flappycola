using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DataBinding
{
    public class ClickBinding : BaseDataBinding, IPointerClickHandler
    {
        [SerializeField] private string _path;

        private IDataSource _dataSource;

        protected override void GatherPathToSubscribe(List<string> result)
        {
        }

        protected override void BindData(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_dataSource != null && _dataSource.TryGetNodeByPath<ICommandProperty>(_path, out var commandProperty))
            {
                commandProperty.Execute(_dataSource);
            }
        }
    }
}