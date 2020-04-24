using Assets.Scripts.BISDSystem.Base;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BISDSystem
{
    [Serializable]
    public abstract class EntityInstance<DATA , STATE> 
        where DATA : EntityData 
        where STATE : IEntityState<DATA>
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
        private STATE state;

        /// <summary>
        /// State accessor readonly
        /// </summary>
        public STATE State { get => state; }

        /// <summary>
        /// State accessor read-write
        /// </summary>
        public ref STATE RefState { get => ref state; }

        public EntityInstance()
        {

        }

        public EntityInstance( STATE state)
        {
            SetState(state);
        }

        public void SetState(STATE state)
        {
            this.state = state;
            OnStateChanged(this.state);
        }

        protected virtual void OnStateChanged(STATE state)
        {

        }

        public void NotifyStateChange()
        {
            OnStateChangedEvent?.Invoke(State);
        }
    }
}
