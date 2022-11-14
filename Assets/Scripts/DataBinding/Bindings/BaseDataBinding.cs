using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    public abstract class BaseDataBinding : MonoBehaviour, IDataObservator
    {
        private bool _hasPendingChanges;
        private readonly List<string> _pathToSubscriptionCache = new List<string>(2);

        public DataContext DataContext { get; private set; }
        protected IDataSource DataSource => DataContext != null ? DataContext.DataSource : null;
        protected virtual bool HandleChangesWhenInactive => false;

        protected virtual void OnEnable()
        {
            if (_hasPendingChanges && !HandleChangesWhenInactive)
            {
                BindData(null);
            }
        }

        protected virtual void OnDisable()
        {}

        public void SetDataContext(DataContext dataContext)
        {
            DataContext = dataContext;
        }

        public void SubscribeToDataSourceChanges()
        {
            if (DataSource != null)
            {
                RebindData();

                _pathToSubscriptionCache.Clear();
                FillPathToSubscription(_pathToSubscriptionCache);

                foreach (var path in _pathToSubscriptionCache)
                {
                    if (!string.IsNullOrEmpty(path) && DataSource.TryGetNodeByPath<IDataProperty>(path, out var property))
                    {
                        property.Subscribe(this);
                    }
                }
            }
        }

        public void UnsubscribeFromDataSourceChanges()
        {
            if (DataSource != null)
            {
                _pathToSubscriptionCache.Clear();
                FillPathToSubscription(_pathToSubscriptionCache);

                foreach (var path in _pathToSubscriptionCache)
                {
                    if (!string.IsNullOrEmpty(path) && DataSource.TryGetNodeByPath<IDataProperty>(path, out var property))
                    {
                        property.Unsubscribe(this);
                    }
                }

                _hasPendingChanges = false;
            }
        }

        protected void RebindData()
        {
            _hasPendingChanges = true;
            BindData(null);
        }

        void IDataObservator.HandleChanges(IDataNode property)
        {
            _hasPendingChanges = true;
            BindData(property);
        }

        private void BindData(IDataNode property)
        {
            if (this == null)
            {
                return;
            }

            if (HandleChangesWhenInactive || gameObject.activeInHierarchy)
            {
                _hasPendingChanges = false;
                BindDataInternal(property);
            }
        }

        protected virtual void BindDataInternal(IDataNode property) {}
        protected virtual void FillPathToSubscription(List<string> result) {}
    }
}