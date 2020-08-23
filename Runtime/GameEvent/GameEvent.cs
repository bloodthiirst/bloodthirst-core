using Bloodthirst.Core.PersistantAsset;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bloodthirst.Core.GameEvent
{
    [SingletonScriptablePath("GameEvent")]
    public abstract class GameEvent<T , TArgs> : SingletonScriptableObject<T> where T : GameEvent<T , TArgs>
    {
        private static event Action<TArgs> subsribers;

        public static void Register(Action<TArgs> action)
        {
            subsribers += action;
        }

        public static void Unregister(Action<TArgs> action)
        {
            subsribers -= action;
        }

        public static void Invoke(TArgs parameter)
        {
            Debug.Log($"GameEvent Invoked :  {typeof(T).GetNiceName()}");

            if (subsribers == null)
                return;

            subsribers?.Invoke(parameter);

        }

        public override void OnGameQuit()
        {
            subsribers = null;
        }
    }
}
