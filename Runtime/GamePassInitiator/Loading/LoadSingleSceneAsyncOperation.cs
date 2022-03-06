using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Scripts.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.Setup
{
    public class LoadSingleSceneAsyncOperation : IAsynOperationWrapper
    {
        public int Order { get; set; }

        private string _scenePath;
        public LoadSingleSceneAsyncOperation(string scenePath)
        {
            _scenePath = scenePath;
        }

        public int OperationsCount()
        {
            return 1;
        }

        public IEnumerable<AsyncOperation> StartOperations()
        {
            AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_scenePath);

            load.completed += OnSceneLoadComplete;
            yield return load;
        }

        private void OnSceneLoadComplete(AsyncOperation obj)
        {
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(_scenePath);

            List<GameObject> sceneGOs = scene.GetRootGameObjects().ToList();

            GameObjectUtils.GetAllComponents<IBeforeAllScenesInitializationPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<ISceneInitializationPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IPostSceneInitializationPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IAfterAllScenesIntializationPass>(sceneGOs, true).ForEach(e => e.Execute()); ;

            GameObjectUtils.GetAllComponents<IQuerySingletonPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IQuerySingletonPass>(sceneGOs, true).ForEach(e => e.Execute()); ;
            GameObjectUtils.GetAllComponents<IInjectPass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IAwakePass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IEnablePass>(sceneGOs, true).ForEach(e => e.Execute());
            GameObjectUtils.GetAllComponents<IPostEnablePass>(sceneGOs, true).ForEach(e => e.Execute());
        }
    }
}
