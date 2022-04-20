using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Bloodthirst.Runtime.BRecorder
{
    public class BRecorderAsset : SerializedScriptableObject
    {
        [field: OdinSerialize]
        public BRecorderSession Session { get; set; }

    }
}
