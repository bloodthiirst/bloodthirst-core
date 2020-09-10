using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    [Serializable]
    public abstract class EntityInstance<DATA, STATE , INSTANCE> : IRemovable
        where DATA : EntityData 
        where STATE : class , IEntityState<DATA>
        where INSTANCE : EntityInstance<DATA, STATE, INSTANCE>
    {
        /// <summary>
        /// Event to listen to instance remove
        /// </summary>
        public Action<INSTANCE> OnInstanceRemoved;

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
        public STATE State {
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

        [Button]
        public virtual void Remove()
        {
            InstanceRegister <INSTANCE>.Unregister((INSTANCE) this);
            OnInstanceRemoved?.Invoke((INSTANCE) this);
        }

        public void NotifyStateChanged()
        {
            OnStateChangedEvent?.Invoke(State);
        }
        public EntityInstance( STATE state)
        {
            State = state;
        }

        protected virtual void OnStateChanged(STATE state)
        {

        }
    }
}
