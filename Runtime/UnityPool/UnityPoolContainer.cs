﻿using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Scripts.Core.UnityPool
{
    [CreateAssetMenu(menuName = "UnityPool/UnityPoolContainer")]
    public class UnityPoolContainer : SerializedScriptableObject
    {
        public Dictionary<MonoBehaviour, int> prefabsPoolDescription;
    }
}
