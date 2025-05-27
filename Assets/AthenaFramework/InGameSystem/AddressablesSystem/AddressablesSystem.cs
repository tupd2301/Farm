using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AthenaFramework.InGameSystem.AddressablesSystem
{
    public class AddressablesSystem : GameSystem
    {
        private Dictionary<string, string> _loadedJsonConfig; // <path, config>
        private List<AsyncOperationHandle<IList<AsyncOperationHandle>>> _loadAssetsHandles; //TODO: ??

        public override void Init()
        {
            _loadedJsonConfig = new();
            _loadAssetsHandles = new();
        }

        public override void Destroy()
        {
            foreach (var loadedAsset in _loadAssetsHandles)
            {
                Addressables.Release(loadedAsset);
            }
        }

        public void LoadAssetAsync<TObject>(List<string> paths, TObject type)
        {
            //
        }

        public void LoadTextAssetAsync(List<string> paths, System.Action onCompleted)
        {
            List<AsyncOperationHandle> loadOperations = new();

            foreach (var path in paths)
            {
                var operationHandle = Addressables.LoadAssetAsync<TextAsset>(path);
                operationHandle.Completed += (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        _loadedJsonConfig[path] = handle.Result.text;
                    }
                    else
                    {
                        Debug.LogError($"Failed to load JSON from {path}");
                    }
                };

                loadOperations.Add(operationHandle);
            }

            var loadAssetsHandle = Addressables.ResourceManager.CreateGenericGroupOperation(loadOperations);
            loadAssetsHandle.Completed += (handles) =>
            {
                onCompleted?.Invoke();
            };

            _loadAssetsHandles.Add(loadAssetsHandle);
        }

        public string GetTextAsset(string path)
        {
            if (_loadedJsonConfig.ContainsKey(path))
            {
                return _loadedJsonConfig[path];
            }

            return null;
        }
    }
}
