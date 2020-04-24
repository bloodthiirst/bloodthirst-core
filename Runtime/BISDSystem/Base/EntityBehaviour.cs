using Assets.Scripts.BISDSystem.Base;
using Assets.Scripts.Core.UnityPool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.BISDSystem
{
    public abstract class EntityBehaviour<DATA, STATE,INSTANCE> : MonoBehaviour
        where DATA : EntityData
        where STATE : IEntityState<DATA>
        where INSTANCE : EntityInstance<DATA,STATE>
    {
        [SerializeField]
        private INSTANCE instance;

        public INSTANCE Instance => instance;

        public DATA Data => instance.State.Data;

        public STATE State => instance.State;

        public ref STATE RefState => ref instance.RefState;

        public void SetInstance(INSTANCE instance)
        {
            this.instance = instance;
            OnSetInstance(this.instance);
        }

        public abstract void OnSetInstance(INSTANCE instance);

        /// <summary>
        /// Takes a state as a parameter and returns a behaviour that has been hooked up using the DATA,STATE,INSTANCE pattern
        /// </summary>
        /// <typeparam name="BEHAVIOUR">Behaviour class</typeparam>
        /// <typeparam name="INSTANCE">Instance class</typeparam>
        /// <typeparam name="STATE">State struct</typeparam>
        /// <param name="state"></param>
        /// <returns>Loaded bhaviour form pool</returns>
        public static BEHAVIOUR Load<BEHAVIOUR>(STATE state) where BEHAVIOUR : EntityBehaviour<DATA, STATE, INSTANCE>
        {
            BEHAVIOUR behaviour = UnityPool.Instance.Get<BEHAVIOUR>();

            behaviour.Instance.SetState(state);

            behaviour.SetInstance(behaviour.Instance);

            return behaviour;
        }

    }
}
