using UnityEngine;

namespace Bloodthirst.Core.SceneManager
{
    public static class SceneManagerExtensions
    {
        /// <summary>
        /// Change the gameObject from one scene to another
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="go"></param>
        public static void ChangeScenes(ISceneInstanceManager from, ISceneInstanceManager to, GameObject go)
        {
            from.RemoveFromScene(go);
            to.AddToScene(go);
        }
    }
}
