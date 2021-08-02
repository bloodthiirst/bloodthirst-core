using Bloodthirst.Core.BISDSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.Core.BISDSystem
{
    public abstract class EntityLoadSaveHandlerBase<TSave, TState> :
    IEntitySaver<TSave, TState>,
    IEntityLoader<TSave, TState>
    where TState : IEntityState
    where TSave : IEntityGameData<TState>
    {
        private static readonly Type saveType = typeof(TSave);
        private static readonly Type stateType = typeof(TState);
        public Type SaveType => saveType;
        public Type StateType => stateType;

        Type IEntityLoader.From => saveType;
        Type IEntityLoader.To => stateType;

        Type IEntitySaver.From => stateType;
        Type IEntitySaver.To => saveType;

        public abstract TSave GetSave(TState save, SavingContext context);
        public abstract TState GetState(TSave save, LoadingContext context);
        public abstract void LinkReferences(TSave save, TState state, LoadingContext context);

        TSave IEntitySaver<TSave, TState>.GetSave(TState state, SavingContext context)
        {
            return GetSave(state , context);
        }

        TState IEntityLoader<TSave, TState>.GetState(TSave save, LoadingContext context)
        {
            return GetState(save, context);
        }
        void IEntityLoader<TSave, TState>.LinkReferences(TSave save, TState state, LoadingContext context)
        {
            LinkReferences(save, state, context);
        }

        IEntityGameData IEntitySaver.GetSave(IEntityState state, SavingContext context)
        {
            return GetSave((TState)state, context);
        }

        IEntityState IEntityLoader.GetState(IEntityGameData save, LoadingContext context)
        {
            return GetState((TSave)save, context);
        }


        void IEntityLoader.LinkReferences(IEntityGameData save, IEntityState state, LoadingContext context)
        {
            LinkReferences((TSave) save ,(TState) state, context);
        }
    }

    public class SavingContext
    {

    }

    public class LoadingContext
    {
        public struct InstanceGameDataPair
        {
            public IEntityInstance Instance { get; set; }
            public IEntityGameData GameData { get; set; }
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