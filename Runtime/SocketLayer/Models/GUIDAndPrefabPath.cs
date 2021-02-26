using Sirenix.OdinInspector;
using System;

namespace Bloodthirst.Models
{
    [Serializable]
    public struct GUIDAndPrefabPath
    {
        [ShowInInspector]
        public Guid Guid { get; set; }

        [ShowInInspector]
        public string PrefabPath { get; set; }
    }
}
