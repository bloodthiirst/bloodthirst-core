﻿using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Core.BISDSystem
{
    public abstract class EntityInstance<DATA, STATE, INSTANCE> : IEntityInstance, ISavable
        where DATA : EntityData
        where STATE : class, IEntityState<DATA>
        where INSTANCE : EntityInstance<DATA, STATE, INSTANCE>
    {

        private readonly Type stateType = typeof(STATE);

        private readonly Type instanceType = typeof(INSTANCE);

        #region ISavable
        Type ISavable.SavableStateType => stateType;

        ISavableIdentifier ISavable.GetIdentifierInfo()
        {
            return EntityIdentifier;
        }

        ISavableState ISavable.GetSavableState()
        {
            return State;
        }

        void ISavable.ApplyState(ISavableState state)
        {
            State = (STATE)state;
        }
        #endregion

        [SerializeField]
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

                if (entityIdentifier == null)
                    return;

                entityIdentifier.OnEntitySpawned -= OnEntitySpawned;
                entityIdentifier.OnEntitySpawned += OnEntitySpawned;

                entityIdentifier.OnEntityRemoved -= OnEntityRemoved;
                entityIdentifier.OnEntityRemoved += OnEntityRemoved;

            }
        }

        [SerializeField]
        private bool isActive = true;

        /// <summary>
        /// Is this instance active ?
        /// </summary>
        public bool IsActive
        {
            get => isActive;
            private set
            {
                if (isActive == value)
                    return;

                isActive = value;
                OnIsActiveChanged?.Invoke((INSTANCE)this);
            }
        }

        public IEntityInstanceProvider InstanceProvider { get; set; }


        public event Action<INSTANCE> OnIsActiveChanged;

        /// <summary>
        /// Event to listen to instance remove
        /// </summary>
        public event Action<INSTANCE> BeforeEntityRemoved;

        /// <summary>
        /// Event to listen to instance spawned
        /// </summary>
        public event Action<INSTANCE> OnEntitySpawn;

        /// <summary>
        /// Event to listen to instance is linked to a behaviour (on instance change or spawn)
        /// </summary>
        public event Action<INSTANCE> OnInstanceBinded;

        /// <summary>
        /// Event to listen to instance disposal/change
        /// </summary>
        public event Action<INSTANCE> OnInstanceDisposed;

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

        public EntityInstance()
        {

        }

        [Button]
        public void Enable()
        {
            IsActive = true;
        }

        [Button]
        public void Disable()
        {
            IsActive = false;
        }

        private void OnEntitySpawned(EntityIdentifier entityIdentifier)
        {
            OnEntitySpawn?.Invoke((INSTANCE)this);
        }

        private void OnEntityRemoved(EntityIdentifier entityIdentifier)
        {
            BeforeEntityRemoved?.Invoke((INSTANCE)this);
        }

        public void NotifyInstanceDisposed()
        {
            BeforeInstanceDisposed();
            OnInstanceDisposed?.Invoke((INSTANCE)this);
        }

        public void NotifyInstanceBinded()
        {
            OnInstanceBinded?.Invoke((INSTANCE)this);
            AfterInstanceBinded();
        }

        protected virtual void BeforeInstanceDisposed()
        {

        }

        protected virtual void AfterInstanceBinded()
        {

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

        #region interface implementations
        EntityIdentifier IEntityInstance.EntityIdentifier => EntityIdentifier;

        IEntityState IEntityInstance.State
        {
            get => State;
            set
            {
                Assert.IsTrue(value is STATE);
                State = (STATE)value;
            }
        }

        Type IEntityInstance.StateType => stateType;

        Type IEntityInstance.InstanceType => instanceType;

        #endregion
    }
}
