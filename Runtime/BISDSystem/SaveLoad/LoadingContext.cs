using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.Core.BISDSystem
{
    public class LoadingContext
    {
        public struct InstanceGameDataPair
        {
            public IEntityInstance Instance { get; set; }
            public IEntityGameSave GameData { get; set; }
        }
        private Dictionary<Type, List<IEntityInstance>> instancesPerType = new Dictionary<Type, List<IEntityInstance>>();

        public IEnumerable<IEntityInstance> AllInstances()
        {
            foreach(KeyValuePair<Type, List<IEntityInstance>> kv in instancesPerType)
            { 
                foreach(IEntityInstance v in kv.Value)
                {
                    yield return v;
                }
            }
        }

        public void AddInstance(IEntityInstance instance)
        {
            Type t = instance.InstanceType;
            
            if(!instancesPerType.TryGetValue(t, out List<IEntityInstance> inst))
            {
                inst = new List<IEntityInstance>();
                instancesPerType.Add(t, inst);
            }

            inst.Add(instance);
        }

        public T GetInstance<T>(int id) where T : IEntityInstance
        {
            Type t = typeof(T);
            instancesPerType.TryGetValue(t, out List<IEntityInstance> inst);
            return (T) inst.FirstOrDefault(i => i.EntityIdentifier.Id == id);
        }
    }
}