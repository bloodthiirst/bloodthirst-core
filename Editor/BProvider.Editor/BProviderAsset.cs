#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
	using Sirenix.Serialization;
#endif

using UnityEngine;

namespace Bloodthirst.Core.BProvider.Editor
{
#if ODIN_INSPECTOR
    public class BProviderAsset : SerializedScriptableObject
#else
    public class BProviderAsset : ScriptableObject
#endif
    {
        
#if ODIN_INSPECTOR[OdinSerialize]
#endif
        public BProvider bProvider = new BProvider();


        
#if ODIN_INSPECTOR[Button]
#endif
        public void Add(UnityEngine.Object obj)
        {
            System.Type t = obj.GetType();
            bProvider.RegisterInstance(t, obj);
        }
    }






}