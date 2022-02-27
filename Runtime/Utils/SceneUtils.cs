using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.Utils
{
    public static class SceneUtils
    {
        public static List<Scene> GetAllScenesInHierarchy(out Scene activeSceneIndex)
        {
            List<Scene> allScenes = new List<Scene>();

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                Scene s = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                allScenes.Add(s);
            }

            activeSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            return allScenes;
        }
    }
}
