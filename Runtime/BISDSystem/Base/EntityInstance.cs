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
                if (entityIdentifier != null)
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
        /// Event to listen to instance is linked to a behaviour (on instance change or spawn)
        /// </summary>
        public event Action<INSTANCE> OnInstanceBinded;

        /// <summary>
        /// Event to listen to instance remove
        /// </summary>
        public event Action<INSTANCE> BeforeEntityRemoved;

        /// <summary>
        /// Event to listen to instance disposal/change
        /// </summary>
        public event Action<INSTANCE> BeforeInstanceDisposed;

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
            BeforeEntityRemoved?.Invoke((INSTANCE)this);
        }

        public void NotifyInstanceDisposed()
        {
            BeforeInstanceDisposed?.Invoke((INSTANCE)this);
        }

        public void NotifyInstanceBinded()
        {
            OnInstanceBinded?.Invoke((INSTANCE)this);
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
