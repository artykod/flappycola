using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DataBinding
{
    public class AddressableAssetBinding : BaseDataBinding
    {
        [SerializeField] private string _path;

        private GameObject _assetInstance;

        protected override void GatherPathToSubscribe(List<string> result)
        {
            result.Add(_path);
        }

        protected override void BindData(IDataSource dataSource)
        {
            if (_assetInstance != null)
            {
                Destroy(_assetInstance);

                _assetInstance = null;
            }

            if (dataSource != null && dataSource.TryGetNodeByPath<IDataProperty>(_path, out var valueProperty))
            {
                var assetKey = valueProperty.GetValue<string>();

                if (!string.IsNullOrEmpty(assetKey))
                {
                    Addressables.LoadAssetAsync<GameObject>(assetKey).Completed += AssetLoadComplete;
                }
            }
        }

        private void AssetLoadComplete(AsyncOperationHandle<GameObject> op)
        {
            op.Completed -= AssetLoadComplete;

            if (_assetInstance != null)
            {
                Destroy(_assetInstance);
            }

            _assetInstance = Instantiate(op.Result, transform, false);
        }
    }
}