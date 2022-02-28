using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.Core.BISDSystem
{
    public class LoadingContext
    {

        private Dictionary<Type, List<ISavable>> instancesPerType = new Dictionary<Type, List<ISavable>>();

        public IEnumerable<ISavable> AllInstances()
        {
            foreach(KeyValuePair<Type, List<ISavable>> kv in instancesPerType)
            { 
                foreach(ISavable v in kv.Value)
                {
                    yield return v;
                }
            }
        }

        public void AddInstance(ISavable instance)
        {
            Type t = instance.GetType();
            
            if(!instancesPerType.TryGetValue(t, out List<ISavable> inst))
            {
                inst = new List<ISavable>();
                instancesPerType.Add(t, inst);
            }

            inst.Add(instance);
        }

        public T GetInstance<T>(Predicate<T> condition) where T : ISavable
        {
            Type t = typeof(T);
            instancesPerType.TryGetValue(t, out List<ISavable> inst);
            return (T) inst.Cast<T>().FirstOrDefault(i => condition(i));
        }
    }
}