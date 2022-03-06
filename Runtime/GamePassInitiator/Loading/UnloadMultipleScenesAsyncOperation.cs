using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.Setup
{
    public class UnloadMultipleScenesAsyncOperation : IAsynOperationWrapper
    {
        public int Order { get; set; }

        private List<string> _scenes;
        public UnloadMultipleScenesAsyncOperation()
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
                Scene s = UnityEngine.SceneManagement.SceneManager.GetSceneByName(scene);

                if (!s.IsValid() || !s.isLoaded)
                    continue;

                yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(s);
            }
        }
    }
}
