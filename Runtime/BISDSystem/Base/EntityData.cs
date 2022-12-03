#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif

using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
#if ODIN_INSPECTOR
    public abstract class EntityData : SerializedScriptableObject
#else
    public abstract class EntityData : ScriptableObject
#endif
    {
    }
}
