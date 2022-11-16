using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    public class DataContext : MonoBehaviour
    {
        private static readonly List<BaseDataBinding> GatherBindingsCache = new List<BaseDataBinding>(8);

        private bool _isBindingsGathered;
        private readonly List<BaseDataBinding> _gatheredBindings = new List<BaseDataBinding>();

        public IDataSource DataSource { get; private set; }

        public void SetDataSource(IDataSource dataSource)
        {
            if (!_isBindingsGathered)
            {
                _isBindingsGathered = true;
                GatherBindingsRecursive(transform);
            }

            if (DataSource == dataSource)
            {
                return;
            }

            DataSource = dataSource;

            foreach (var i in _gatheredBindings)
            {
                i.SetDataContext(this);
            }
        }

        private void GatherBindingsRecursive(Transform root)
        {
            GatherBindingsFromTransform(root);

            for (int i = 0, l = root.childCount; i < l; ++i)
            {
                var child = root.GetChild(i);

                if (child.GetComponent<DataContext>() != null)
                {
                    continue;
                }

                GatherBindingsFromTransform(child);
                GatherBindingsRecursive(child);
            }
        }

        private void GatherBindingsFromTransform(Transform target)
        {
            target.GetComponents(GatherBindingsCache);

            _gatheredBindings.AddRange(GatherBindingsCache);

            GatherBindingsCache.Clear();
        }
    }   
}