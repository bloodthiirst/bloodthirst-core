using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Bloodthirst.Core.BProvider.Editor
{
    public class BProviderAsset : SerializedScriptableObject
    {
        [OdinSerialize]
        public BProvider bProvider = new BProvider();


        [Button]
        public void Add(UnityEngine.Object obj)
        {
            System.Type t = obj.GetType();
            bProvider.RegisterInstance(t, obj);
        }
    }
}