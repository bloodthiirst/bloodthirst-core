using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public class InstanceRegister<INSTANCE> where INSTANCE : IEntityInstance
    {
        private static HashSet<IEntityInstance> instances;

        private static HashSet<IEntityInstance> Instances
        {
            get
            {
                if (instances == null)
                {
                    instances = new HashSet<IEntityInstance>();
                    EntityManager._AllInstanceSets.Add(instances);

                }

                return instances;
            }
        }

        /// <summary>
        /// List of the instances alive
        /// </summary>
        public static IReadOnlyCollection<IEntityInstance> AvailableInstances => Instances;


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
