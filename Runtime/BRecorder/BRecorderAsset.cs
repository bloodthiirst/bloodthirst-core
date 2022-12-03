#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
#if ODIN_INSPECTOR
	using Sirenix.Serialization;
#endif

using UnityEngine;

namespace Bloodthirst.Runtime.BRecorder
{
#if ODIN_INSPECTOR
    public class BRecorderAsset : SerializedScriptableObject
#else
    public class BRecorderAsset : ScriptableObject
#endif
    {
        public class Child
        {
            public int health;

            public Child child;
        }

#if ODIN_INSPECTOR
        [field: OdinSerialize]
#endif
        public BRecorderSession Session { get; set; }

        
#if ODIN_INSPECTOR
[OdinSerialize]
#endif

        public Child child;

    }
}
