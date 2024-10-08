using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.Utils
{
    public static class SceneUtils
    {
        public static void GetAllScenesInHierarchy(out Scene activeSceneIndex, List<Scene> allScenes)
        {

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                Scene s = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                allScenes.Add(s);
            }

            activeSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        }
    }
}
