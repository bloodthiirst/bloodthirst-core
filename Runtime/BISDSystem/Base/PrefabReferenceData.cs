#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    [CreateAssetMenu(menuName = "BISD/Prefab Reference/Data")]
    public class PrefabReferenceData : ScriptableObject
    {
        [SerializeField]
#if ODIN_INSPECTOR
        [PreviewField(200f, ObjectFieldAlignment.Right)]
#endif
        private GameObject prefabReference;

        public GameObject PrefabReference => prefabReference;
    }
}
