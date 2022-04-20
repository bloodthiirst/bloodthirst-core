using Bloodthirst.BJson;
using UnityEditor;

namespace Bloodthirst.JsonUnityObject
{
    [InitializeOnLoad]
    public static class JsonUnityObjectSettings
    {

        /// <summary>
        /// Get a pooled instance of the serialization settings
        /// </summary>
        /// <returns></returns>
        public static BJsonSettings GetSettings()
        {
            return new BJsonSettings()
            {
#if UNITY_EDITOR
                Formated = true
#else
               Formated = false
#endif
            };
        }
    }
}
