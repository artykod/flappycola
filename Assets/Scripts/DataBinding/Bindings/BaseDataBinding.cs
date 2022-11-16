using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    public abstract class BaseDataBinding : MonoBehaviour, IDataObservator
    {
        private bool _isInited;
        private bool _isChanged;
        private IDataSource _dataSource;
        private readonly List<string> _pathToSubscription = new List<string>(2);

        public DataContext DataContext { get; private set; }

        protected virtual bool HandleChangesWhenInactive => false;

        public void SetDataContext(DataContext dataContext)
        {
            if (!_isInited)
            {
                _isInited = true;

                GatherPathToSubscribe(_pathToSubscription);
            }

            DataContext = dataContext;

            var dataSource = DataContext != null ? DataContext.DataSource : null;

            if (_dataSource == dataSource)
            {
                return;
            }

            UnsubscribeFromDataSourceChanges();

            _dataSource = dataSource;

            SubscribeToDataSourceChanges();

            BindData(_dataSource);
        }

        protected virtual void Awake()
        {
            if (DataContext == null)
            {
                SetDataContext(GetComponentInParent<DataContext>(true));
            }
        }

        protected virtual void OnEnable()
        {
            if (!HandleChangesWhenInactive)
            {
                SubscribeToDataSourceChanges();
            }
        }

        protected virtual void Update()
        {
            if (_isChanged)
            {
                _isChanged = false;

                BindData(_dataSource);
            }
        }

        protected virtual void OnDisable()
        {
            if (!HandleChangesWhenInactive)
            {
                UnsubscribeFromDataSourceChanges();
            }
        }

        void IDataObservator.HandleChanges(IDataNode property)
        {
            if (HandleChangesWhenInactive)
            {
                BindData(_dataSource);
            }
            else
            {
                _isChanged = true;
            }
        }

        private void SubscribeToDataSourceChanges()
        {
            if (_dataSource == null)
            {
                return;
            }

            foreach (var path in _pathToSubscription)
            {
                if (!string.IsNullOrEmpty(path) && _dataSource.TryGetNodeByPath<IDataNode>(path, out var property))
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

            foreach (var path in _pathToSubscription)
            {
                if (!string.IsNullOrEmpty(path) && _dataSource.TryGetNodeByPath<IDataNode>(path, out var property))
                {
                    property.Unsubscribe(this);
                }
            }
        }

        protected abstract void BindData(IDataSource dataSource);
        protected abstract void GatherPathToSubscribe(List<string> result);
    }
}