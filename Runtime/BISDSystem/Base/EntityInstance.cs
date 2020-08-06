using Assets.Scripts.BISDSystem.Base;
using System;
using UnityEngine;

namespace Assets.Scripts.BISDSystem
{
    [Serializable]
    public abstract class EntityInstance<DATA , STATE> 
        where DATA : EntityData 
        where STATE : class , IEntityState<DATA>
    {

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

        public void NotifyStateChange()
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
