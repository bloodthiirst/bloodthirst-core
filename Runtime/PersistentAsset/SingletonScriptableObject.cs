#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using Bloodthirst.JsonUnityObject;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Core.PersistantAsset
{
#if ODIN_INSPECTOR
    public abstract class SingletonScriptableObject<T> : SerializedScriptableObject, ISingletonScriptableObject where T : SerializedScriptableObject
#else
    public abstract class SingletonScriptableObject<T> : JsonScriptableObject, ISingletonScriptableObject where T : ScriptableObject
#endif
    {
        /// <summary>
        /// <para>Default path to the create the asset at</para>
        /// <para>The path starts directly after "Assets/Resources"</para>
        /// </summary>
        protected static readonly string DefaultPath = "Singletons/" + typeof(T).Name;

        protected static T _instance;

        /// <summary>
        /// Get singleton instance of type <see cref="T"></see>
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    Reload();
                }
                return _instance;
            }
        }

        /// <summary>
        /// <para>The path to the scriptableObject</para>
        /// <para>If an attribe of type <see cref="SingletonScriptablePath"/> is present on the class , then the asset will be generater there</para>
        /// <para>Otherwise , the asset will be created at <see cref="DefaultPath"/></para>
        /// </summary>
        public static string AssetPath
        {
            get
            {
                SingletonScriptablePath pathAttr = (SingletonScriptablePath)typeof(T).GetCustomAttributes(typeof(SingletonScriptablePath), true).FirstOrDefault();

                if (pathAttr == null)
                {
                    return DefaultPath;
                }

                return pathAttr.Path + "/" + typeof(T).Name;
            }
        }

        private static void Reload()
        {
            T resource = Resources.Load<T>(AssetPath);

            if (resource != null)
            {
                _instance = resource;
            }
        }

        public virtual void OnGameQuit()
        {

        }
    }
}
