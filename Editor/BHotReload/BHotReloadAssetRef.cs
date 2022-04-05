using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Editor.BHotReload
{
    public class BHotReloadAssetRef : ScriptableObject
    {
        [SerializeField]
        public List<UnityEngine.Object> unityObjs;
    }
}
