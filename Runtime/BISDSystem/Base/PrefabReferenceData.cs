using Sirenix.OdinInspector;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    [CreateAssetMenu(menuName = "BISD/Prefab Reference/Data")]
    public class PrefabReferenceData : ScriptableObject
    {
        [SerializeField]
        [PreviewField(200f, ObjectFieldAlignment.Right)]
        private GameObject prefabReference;

        public GameObject PrefabReference => prefabReference;
    }
}
