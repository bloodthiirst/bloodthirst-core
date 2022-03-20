using Bloodthirst.System.CommandSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.Setup
{
    public class LoadMultipleScenesAsyncOperation : IAsynOperationWrapper
    {
        private List<string> _scenes;

        int IAsynOperationWrapper.Order { get; }
        
        public LoadMultipleScenesAsyncOperation()
        {
            _scenes = new List<string>();
        }

        int IAsynOperationWrapper.OperationsCount()
        {
            return _scenes.Count;
        }

        IEnumerable<AsyncOperation> IAsynOperationWrapper.StartOperations()
        {
            foreach (string scene in _scenes)
            {
                yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            }
        }

        public void Add(string scenePath)
        {
            _scenes.Add(scenePath);
        }
    }
}
