using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Core.UnityPool
{
    [CreateAssetMenu(menuName ="UnityPool/UnityPoolContainer")]
    public class UnityPoolContainer : SerializedScriptableObject
    {
        public Dictionary<MonoBehaviour, int> prefabsPoolDescription;
    }
}
