using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Scripts.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.Setup
{
    public class UnloadSceneAsyncOperation : IAsynOperationWrapper
    {
        public int Order { get; set; }

        private string _scenePath;
        public UnloadSceneAsyncOperation(string scenePath)
        {
            _scenePath = scenePath;
        }

        public int OperationsCount()
        {
            return 1;
        }

        public IEnumerable<AsyncOperation> StartOperations()
        {
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(_scenePath);

            List<GameObject> gos = scene.GetRootGameObjects().ToList();

            GameObjectUtils.GetAllComponents<IBeforeSceneUnload>(gos, true).ForEach(e => e.Execute());

            AsyncOperation unload = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_scenePath);

            yield return unload;
        }
    }
}
