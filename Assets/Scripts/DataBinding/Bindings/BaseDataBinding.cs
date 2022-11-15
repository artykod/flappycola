using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    public abstract class BaseDataBinding : MonoBehaviour, IDataObservator
    {
        private bool _isInitialized;
        private bool _isNotFirstEnable;
        private IDataSource _dataSource;
        private readonly List<string> _pathToSubscriptionCache = new List<string>(2);

        public DataContext DataContext { get; private set; }
        protected virtual bool HandleChangesWhenInactive => false;

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            GatherPathToSubscribe(_pathToSubscriptionCache);

            TrackDataContext();
        }

        private void TrackDataContext()
        {
            var actualDataContext = GetComponentInParent<DataContext>(true);
            var actualDataSource = actualDataContext != null ? actualDataContext.DataSource : null;

            DataContext = actualDataContext;

            if (_dataSource == actualDataSource)
            {
                return;
            }

            UnsubscribeFromDataSourceChanges();

            _dataSource = actualDataSource;

            SubscribeToDataSourceChanges();

            BindData(_dataSource);
        }

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void OnEnable()
        {
            if (_isNotFirstEnable && !HandleChangesWhenInactive)
            {
                SubscribeToDataSourceChanges();
            }

            _isNotFirstEnable = true;
        }

        protected virtual void Update()
        {
            TrackDataContext();
        }

        protected virtual void OnDisable()
        {
            if (!HandleChangesWhenInactive)
            {
                UnsubscribeFromDataSourceChanges();
            }
        }

        private void SubscribeToDataSourceChanges()
        {
            if (_dataSource == null)
            {
                return;
            }

            foreach (var path in _pathToSubscriptionCache)
            {
                if (!string.IsNullOrEmpty(path) && _dataSource.TryGetNodeByPath<IDataProperty>(path, out var property))
                {
                    property.Subscribe(this);
                }
            }
        }

        private void UnsubscribeFromDataSourceChanges()
        {
            if (_dataSource == null)
            {
                return;
            }

            foreach (var path in _pathToSubscriptionCache)
            {
                if (!string.IsNullOrEmpty(path) && _dataSource.TryGetNodeByPath<IDataProperty>(path, out var property))
                {
                    property.Unsubscribe(this);
                }
            }
        }

        void IDataObservator.HandleChanges(IDataNode property)
        {
            BindData(_dataSource);
        }

        protected virtual void BindData(IDataSource dataSource) {}
        protected virtual void GatherPathToSubscribe(List<string> result) {}
    }
}