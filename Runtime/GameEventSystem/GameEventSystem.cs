using Bloodthirst.Core.EnumLookup;
using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bloodthirst.Core.GameEventSystem
{

    public static class GameEventSystemUtils
    {
        public static bool IsGameEventClass(Type t , out Type enumType)
        {
            Type genericType = typeof(IGameEvent<,>);

            Type hasTheInterface = t.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType);

            if(hasTheInterface == null)
            {
                enumType = null;
                return false;
            }

            enumType = hasTheInterface.GetGenericArguments()[1];
            return true;
        }
    }

    public abstract class GameEventSystem<T> where T : Enum
    {
        private List<Delegate>[] EventSubscriptions { get; set; }

        private const string INIT_METHOD_NAME = "Init";
        private bool isInit;

        public GameEventSystem()
        {
            EventSubscriptions = new List<Delegate>[EnumUtils<T>.EnumCount];

            for (int i = 0; i < EnumUtils<T>.EnumCount; i++)
            {
                EventSubscriptions[i] = new List<Delegate>();
            }

            if (isInit)
                return;

            InitializeIDs();
        }


        private void InitializeIDs()
        {
            Type enumType = typeof(T);
            

            List<Type> all = TypeUtils.AllTypes
                    .Where(t => t.IsClass)
                    .Where(t => GameEventSystemUtils.IsGameEventClass(t , out _))
                    .ToList();

            for (int i = 0; i < all.Count; i++)
            {
                Type type = all[i];

                MethodInfo method = type.GetMethod(INIT_METHOD_NAME, BindingFlags.Static | BindingFlags.NonPublic);

                method.Invoke(null, Array.Empty<object>());
            }
        }

        public void Listen<TEvent>(Action<TEvent> listen) where TEvent : IGameEvent<TEvent, T>
        {
            int index = (int)(object)IGameEvent<TEvent, T>.EventID;

            EventSubscriptions[index].Add(listen);
        }

        public void Unlisten<TEvent>(Action<TEvent> listen) where TEvent : IGameEvent<TEvent, T>
        {
            int index = (int)(object)IGameEvent<TEvent, T>.EventID;

            EventSubscriptions[index].Remove(listen);
        }

        public void Trigger<TEvent>(TEvent eventArgs) where TEvent : IGameEvent<TEvent, T>
        {
            int index = (int)(object)IGameEvent<TEvent, T>.EventID;

            for (int i = 0; i < EventSubscriptions[index].Count; i++)
            {
                Delegate curr = EventSubscriptions[index][i];

                ((Action<TEvent>)curr).Invoke(eventArgs);
            }
        }

    }
}