using Bloodthirst.BJson;
using Bloodthirst.Runtime.BInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Bloodthirst.Core.Utils;

#if ODIN_INSPECTOR
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using Bloodthirst.Core.Utils;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bloodthirst.JsonUnityObject
{
    public abstract class JsonScriptableObject : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Contains the actual JSON representation of the object
        /// </summary>
        [BJsonIgnore]
#if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowIf(nameof(showJsonData))]
#endif
        [BInspectorIgnore]
        [SerializeField]
        [TextArea(5, 20)]
        protected string jsonData;

        /// <summary>
        /// Contains all the unity objects references that should remain serialized by unity
        /// </summary>
        [BJsonIgnore]
#if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowIf(nameof(showJsonData))]
#endif
        [BInspectorIgnore]
        [SerializeField]
        protected List<UnityEngine.Object> unityObjects;

#if UNITY_EDITOR
        /// <summary>
        /// Toggle to see the JSON result for debugging puposes
        /// </summary>
        [BInspectorIgnore]
        [SerializeField]
        protected bool showJsonData;
#endif

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            try
            {
                JsonUnityObjectUtils.DeserializeUnityObject(jsonData, this, unityObjects);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }


        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            try
            {
                string oldJson = jsonData;
                unityObjects = unityObjects.CreateOrClear();
                string newJson = JsonUnityObjectUtils.SerializeUnityObject(this, unityObjects);

#if UNITY_EDITOR
                if (!string.Equals(oldJson, newJson))
                {
                    //Undo.RecordObject(this, $"{gameObject.name} Data change on component {GetType().Name}");
                    jsonData = newJson;
                    EditorUtility.SetDirty(this);
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogException(ex, this);

            }
        }

        [BButton]
#if ODIN_INSPECTOR
        
#if ODIN_INSPECTOR
[Button]
#endif

#endif
        private void LogJson()
        {
            Debug.Log(jsonData);
        }
    }
}
