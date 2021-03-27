using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    [CreateAssetMenu(menuName = "BISD/Prefab Reference/Data")]
    public class PrefabReferenceData : ScriptableObject
    {
        [SerializeField]
        private GameObject prefabReference;

        public GameObject PrefabReference => prefabReference;
    }
}
