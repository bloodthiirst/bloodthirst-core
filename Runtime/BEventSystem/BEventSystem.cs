using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodthirst.BEventSystem
{
    public class BEventSystem<TEventBase>
    {
        private static Type EventBaseType => typeof(TEventBase);

        private Dictionary<Type, List<Delegate>> EventSubsriptions { get; set; }

        public BEventSystem()
        {
            EventSubsriptions = new Dictionary<Type, List<Delegate>>();

            // pre-initialize the dictionary to avoid braching
            IEnumerable<Type> eventTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, EventBaseType))
                .ToList();

            foreach(Type t in eventTypes)
            {
                EventSubsriptions.Add(t, new List<Delegate>());
            }
        }

        /// <summary>
        /// Dispatch an event of type <typeparamref name="T"/> to all the listeners
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventArgs"></param>
        public void Trigger<T>(T eventArgs) where T : TEventBase
        {
            Type eventType = typeof(T);

            List<Delegate> list = EventSubsriptions[eventType];

            foreach(Delegate callback in list)
            {
                callback.DynamicInvoke(eventArgs);
            }
        }

        /// <summary>
        /// Listen for events of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventArgs"></param>
        public void Listen<T>(Action<T> callback) where T : TEventBase
        {
            Type eventType = typeof(T);

            List<Delegate> list = EventSubsriptions[eventType];

            list.Add(callback);
        }

        /// <summary>
        /// Stop listening to events of type <typeparamref name="T"/> to all the listeners
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        public void Unlisten<T>(Action<T> callback) where T : TEventBase
        {
            Type eventType = typeof(T);

            List<Delegate> list = EventSubsriptions[eventType];

            list.Remove(callback);
        }

        public void UnlistenAll()
        {
            foreach(KeyValuePair<Type, List<Delegate>> kv in EventSubsriptions)
            {
                kv.Value.Clear();
            }
        }

        public void UnlistenAll<T>() where T : TEventBase
        {
            Type eventType = typeof(T);

            List<Delegate> list = EventSubsriptions[eventType];

            list.Clear();
        }
    }
}
