using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Bloodthirst.Runtime.BRecorder
{
    public class BRecorderAsset : SerializedScriptableObject
    {
        public class Child
        {
            public int health;

            public Child child;
        }

        [field: OdinSerialize]
        public BRecorderSession Session { get; set; }

        [OdinSerialize]
        public Child child;

    }
}
