using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    public class ListBinding : BaseDataBinding
    {
        [SerializeField] private string _listPath;
        [SerializeField] private GameObject _template;
        [SerializeField] private DataSourceFilter _filter;

        private readonly List<DataContext> _items = new List<DataContext>();
        private readonly List<IDataSource> _filterCache = new List<IDataSource>();

        protected override void FillPathToSubscription(List<string> result)
        {
            result.Add(_listPath);
        }

        protected override void BindDataInternal(IDataNode property)
        {
            if (_template != null)
            {
                _template.SetActive(false);
            }

            foreach (var item in _items)
            {
                Destroy(item.gameObject);
            }

            _items.Clear();

            if (!DataSource.TryGetNodeByPath<IDataSource>(_listPath, out var listDataSource))
            {
                return;
            }

            if (_template == null)
            {
                return;
            }

            _filterCache.Clear();

            listDataSource.ForEach<IDataSource>(i =>
            {
                if (_filter.IsMatch(i))
                {
                    _filterCache.Add(i);
                }
            });

            for (var i = _filterCache.Count - 1; i >= 0; --i)
            {
                AddItemView(_filterCache[i]);
            }
        }

        private void AddItemView(IDataSource dataSource)
        {
            if (_template == null)
            {
                return;
            }

            var templateDataContext = _template.GetComponent<DataContext>();

            if (templateDataContext == null)
            {
                return;
            }

            var instance = Instantiate(templateDataContext, transform, false);
            var instanceTransform = instance.transform;

            instanceTransform.SetAsFirstSibling();
            instanceTransform.localScale = Vector3.one;
            instanceTransform.localPosition = Vector3.zero;
            instanceTransform.localRotation = Quaternion.identity;

            instance.SetDataSource(dataSource);

#if UNITY_EDITOR
            var instanceName = instance.gameObject.name;
            instance.gameObject.name = instanceName.Replace("(Clone)", $"({dataSource.Name})");
#endif

            instance.gameObject.SetActive(true);

            _items.Add(instance);
        }
    }
}