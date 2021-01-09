using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    [Serializable]
    public abstract class EntityInstance<DATA, STATE, INSTANCE>
        where DATA : EntityData
        where STATE : class, IEntityState<DATA>
        where INSTANCE : EntityInstance<DATA, STATE, INSTANCE>
    {
        private EntityIdentifier entityIdentifier;

        public EntityIdentifier EntityIdentifier
        {
            get => entityIdentifier;
            set
            {
                // unsubscribe old events
                if(entityIdentifier != null)
                {
                    entityIdentifier.OnEntitySpawned -= OnEntitySpawned;
                    entityIdentifier.OnEntityRemoved -= OnEntityRemoved;
                }

                entityIdentifier = value;

                entityIdentifier.OnEntitySpawned -= OnEntitySpawned;
                entityIdentifier.OnEntitySpawned += OnEntitySpawned;

                entityIdentifier.OnEntityRemoved -= OnEntityRemoved;
                entityIdentifier.OnEntityRemoved += OnEntityRemoved;

            }
        }

        /// <summary>
        /// Event to listen to instance remove
        /// </summary>
        public Action<INSTANCE> BeforeEntityRemoved;

        /// <summary>
        /// Event to listen to state changes
        /// </summary>
        public event Action<STATE> OnStateChangedEvent;

        /// <summary>
        /// Data accessor
        /// </summary>
        public DATA Data { get => state.Data; }

        /// <summary>
        /// State field
        /// </summary>
        [SerializeField]
        protected STATE state;

        /// <summary>
        /// State accessor
        /// </summary>
        public STATE State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                OnStateChanged(state);
                OnStateChangedEvent?.Invoke(state);
            }
        }


        public IInstanceProvider InstanceProvider { get; set; }

        public EntityInstance()
        {

        }

        private void OnEntitySpawned(EntityIdentifier entityIdentifier)
        {

        }

        private void OnEntityRemoved(EntityIdentifier entityIdentifier)
        {
            InstanceRegister<INSTANCE>.Unregister((INSTANCE)this);
            BeforeEntityRemoved?.Invoke((INSTANCE)this);
        }

        public void NotifyStateChanged()
        {
            OnStateChangedEvent?.Invoke(State);
        }
        public EntityInstance(STATE state)
        {
            State = state;
        }

        protected virtual void OnStateChanged(STATE state)
        {

        }
    }
}
