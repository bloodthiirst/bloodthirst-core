using Bloodthirst.JsonUnityObject;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Bloodthirst.Runtime.BRecorder
{
    public class BRecorderAsset : SerializedScriptableObject
    {
        [field:  OdinSerialize]
        public BRecorderSession Session { get; set; }

    }
}
