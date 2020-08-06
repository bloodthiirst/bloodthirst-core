using Assets.Scripts.BISDSystem.Base;
using Assets.Scripts.Core.UnityPool;
using Packages.BloodthirstCore.Runtime.BISDSystem.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.BISDSystem
{
    public enum LOAD_METHOD
    {
        FROM_DATA,
        FROM_INSTANCE
    }

    public abstract class EntityBehaviour<DATA, STATE,INSTANCE> : MonoBehaviour , IInitializeInstance , IRegisterInstance , IInitializeProvider
        where DATA : EntityData
        where STATE : class , IEntityState<DATA> , new()
        where INSTANCE : EntityInstance<DATA,STATE> , new()
    {

        public INSTANCE Instance => instance;

        public DATA Data => instance.State.Data;

        public STATE State => instance.State;

        [SerializeField]
        private LOAD_METHOD loadMethod = default;

        [SerializeField]
        [ShowIf("loadMethod", LOAD_METHOD.FROM_DATA)]
        private DATA loadData = default;

        public DATA TagData => loadData;

        [SerializeField]
        [ShowIf("loadMethod", LOAD_METHOD.FROM_INSTANCE)]
        private INSTANCE instance = default;

        public void SetInstance(INSTANCE instance)
        {
            this.instance = instance;

            OnSetInstance(this.instance);

            instance.NotifyStateChange();
        }

        /// <summary>
        /// Inject the instance either by using the exterior data or by using the serialized instance
        /// </summary>
        public virtual void InitializeInstance()
        {
            switch (loadMethod)
            {
                case LOAD_METHOD.FROM_DATA:
                    {
                        STATE st = new STATE();
                        st.Data = loadData;

                        INSTANCE ins = new INSTANCE();
                        ins.State = st;

                        SetInstance(ins);
                        break;
                    }
                case LOAD_METHOD.FROM_INSTANCE:
                    {
                        SetInstance(Instance);
                        break;
                    }

                default:
                    break;
            }
        }

        public void RegisterInstance(IInstanceRegister instanceRegister)
        {
            instanceRegister.Register(Instance);
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

            behaviour.Instance.State = state;

            behaviour.SetInstance(behaviour.Instance);


            return behaviour;
        }

        public void InitializeProvider(IInstanceProvider instanceProvider)
        {
            Instance.InstanceProvider = instanceProvider;
        }
    }
}
