using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Scripts.Core.UnityPool
{
    [CreateAssetMenu(menuName = "UnityPool/UnityPoolContainer")]
    public class GenericUnityPoolContainer : SerializedScriptableObject
    {
        public Dictionary<MonoBehaviour, int> prefabsPoolDescription;
    }
}
