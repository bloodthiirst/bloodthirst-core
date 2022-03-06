using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.Setup
{
    public class LoadMultipleScenesAsyncOperation : IAsynOperationWrapper
    {
        public int Order { get; set; }

        private List<string> _scenes;
        public LoadMultipleScenesAsyncOperation()
        {
            _scenes = new List<string>();
        }

        public void Add(string scenePath)
        {
            _scenes.Add(scenePath);
        }

        public int OperationsCount()
        {
            return _scenes.Count;
        }

        public IEnumerable<AsyncOperation> StartOperations()
        {
            foreach (string scene in _scenes)
            {
                yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            }
        }
    }
}
