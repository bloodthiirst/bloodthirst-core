using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.Core.UnitySingleton {
    public class SerializedUnitySingleton<T> : SerializedMonoBehaviour where T : SerializedUnitySingleton<T> {

        protected static T instance;

        [ShowInInspector]
        public static T Instance {
            get {
                if (instance == null) {
                    instance = FindObjectOfType<T>();
                }
                return instance;
            }
        }

        protected virtual void Awake() {
            if (instance == null)
            {
                instance = (T)this;
                return;
            }
            else if (instance == (T)this ){
                return;
            }
            else
            {
                Debug.Log("duplicate singleton found : " + gameObject.name + " and " + instance.name);
                Destroy(gameObject);
            }
        }
    }


}
