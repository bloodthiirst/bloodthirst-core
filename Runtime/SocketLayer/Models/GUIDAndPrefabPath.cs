#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;

namespace Bloodthirst.Models
{
    [Serializable]
    public struct GUIDAndPrefabPath
    {
        #if ODIN_INSPECTOR[ShowInInspector]#endif
        public Guid Guid { get; set; }

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        public string PrefabPath { get; set; }
    }
}
