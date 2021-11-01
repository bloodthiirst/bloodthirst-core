using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.JsonUnityObject
{
    public abstract class JsonScriptableObject : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Contains the actual JSON representation of the object
        /// </summary>
#if ODIN_INSPECTOR
        [ShowIf(nameof(showJSON))]
#endif
        [SerializeField]
        [JsonIgnore]
        [TextArea(5, 20)]
        private string jsonData;

        /// <summary>
        /// Contains all the unity objects references that should remain serialized by unity
        /// </summary>
        [SerializeField]
        [JsonIgnore]
        private List<UnityEngine.Object> unityObjects;

#if UNITY_EDITOR
        /// <summary>
        /// Toggle to see the JSON result for debugging puposes
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        [JsonIgnore]
        private bool showJSON;
#endif
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            JsonUnityObjectUtils.DeserializeUnityObject(jsonData, this, unityObjects);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            jsonData = JsonUnityObjectUtils.SerializeUnityObject(this, unityObjects);
        }
    }
}
