using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupAlphaBinding : BaseDataBinding
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private string _path;

        protected override void FillPathToSubscription(List<string> result)
        {
            result.Add(_path);
        }

        protected override void BindDataInternal(IDataNode property)
        {
            if (DataSource.TryGetNodeByPath<IDataProperty>(_path, out var valueProperty))
            {
                if (_canvasGroup == null)
                {
                    _canvasGroup = GetComponent<CanvasGroup>();

                    if (_canvasGroup == null)
                    {
                        return;
                    }
                }

                _canvasGroup.alpha = valueProperty.GetValue<float>();
            }
        }
    }
}