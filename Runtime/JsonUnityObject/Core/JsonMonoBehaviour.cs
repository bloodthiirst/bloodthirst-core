using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.JsonUnityObject
{
    public abstract class JsonMonoBehaviour : MonoBehaviour, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Contains the actual JSON representation of the object
        /// </summary>
        [SerializeField]
        [JsonIgnore]
        [TextArea(5, 20)]
        protected string jsonData;

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
        [JsonIgnore]
        private bool showJSON;
#endif

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            JsonUnityObjectUtils.DeserializeUnityObject(jsonData, this, unityObjects);
        }


        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            jsonData = JsonUnityObjectUtils.SerializeUnityObject(this , unityObjects);
        }

    }
}
