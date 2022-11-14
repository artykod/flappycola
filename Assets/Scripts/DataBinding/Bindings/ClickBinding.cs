using UnityEngine;
using UnityEngine.EventSystems;

namespace DataBinding
{
    public class ClickBinding : BaseDataBinding, IPointerClickHandler
    {
        [SerializeField] private string _path;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (DataSource != null && DataSource.TryGetNodeByPath<ICommandProperty>(_path, out var commandProperty))
            {
                commandProperty.Execute(DataSource);
            }
        }
    }
}