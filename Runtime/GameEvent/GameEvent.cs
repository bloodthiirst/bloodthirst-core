using Bloodthirst.Core.PersistantAsset;
using Bloodthirst.Core.Utils;
#if ODIN_INSPECTOR
	using Sirenix.Utilities;
#endif
using System;
using UnityEngine;

namespace Bloodthirst.Core.GameEvent
{
    [SingletonScriptablePath("GameEvent")]
    public abstract class GameEvent<T, TArgs> : SingletonScriptableObject<T> where T : GameEvent<T, TArgs>
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
            Debug.Log($"GameEvent Invoked :  { TypeUtils.GetNiceName(typeof(T))}");

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
