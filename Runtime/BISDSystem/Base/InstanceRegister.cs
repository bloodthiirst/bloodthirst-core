using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public class InstanceRegister<INSTANCE>
    {
        private static HashSet<INSTANCE> instances;

        private static HashSet<INSTANCE> Instances
        {
            get
            {
                if (instances == null)
                {
                    instances = new HashSet<INSTANCE>();
                }

                return instances;
            }
        }

        /// <summary>
        /// List of the instances alive
        /// </summary>
        public static IReadOnlyCollection<INSTANCE> AvailableInstances => Instances;

        /// <summary>
        /// register the instance as an alive instances
        /// </summary>
        /// <param name="i"></param>
        public static void Register(INSTANCE i)
        {
            if (Instances.Add(i))
            {
                Debug.Log($"Instance of { typeof(INSTANCE) } has been registered");
                OnInstanceAdded<INSTANCE>.Invoke(i);
            }
        }

        /// <summary>
        /// unregister the instance from the alive instances
        /// </summary>
        public static void Unregister(INSTANCE i)
        {
            if (Instances.Remove(i))
            {
                Debug.Log($"Instance of { typeof(INSTANCE) } has been unregistered");
                OnInstanceRemoved<INSTANCE>.Invoke(i);
            }
        }
    }
}
