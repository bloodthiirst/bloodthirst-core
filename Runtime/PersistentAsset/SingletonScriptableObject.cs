using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Core.PersistantAsset
{
    public abstract class SingletonScriptableObject<T> : SerializedScriptableObject, ISingletonScriptableObject where T : SerializedScriptableObject
    {
        protected static readonly string DefaultPath = "Singletons/" + typeof(T).Name;

        protected static T _instance;

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

        public static string AssetPath
        {
            get
            {
                SingletonScriptablePath pathAttr = (SingletonScriptablePath) typeof(T).GetCustomAttributes(typeof(SingletonScriptablePath), true).FirstOrDefault();

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
