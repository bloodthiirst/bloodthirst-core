using Assets.Scripts.Core.UnityPool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public enum LOAD_METHOD
    {
        FROM_DATA,
        FROM_INSTANCE
    }

    public abstract class EntityBehaviour<DATA, STATE,INSTANCE> : MonoBehaviour , IInitializeInstance , IRegisterInstance , IInitializeProvider,
        IBehaviour<INSTANCE>,
        IRemovableBehaviour
        where DATA : EntityData
        where STATE : class , IEntityState<DATA> , new()
        where INSTANCE : EntityInstance<DATA,STATE> , new()
    {

        public abstract INSTANCE Instance { get; set; }

        public DATA Data => instance.State.Data;

        public STATE State => instance.State;

        [SerializeField]
        private LOAD_METHOD loadMethod = default;

        [SerializeField]
        [ShowIf("loadMethod", LOAD_METHOD.FROM_DATA)]
        private DATA loadData = default;

        public DATA TagData => loadData;

        public IRemovable Removable => Instance;

        [SerializeField]
        [ShowIf("loadMethod", LOAD_METHOD.FROM_INSTANCE)]
        protected INSTANCE instance = default;

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

                        Instance = ins;
                        break;
                    }
                case LOAD_METHOD.FROM_INSTANCE:
                    {
                        Instance = Instance;
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

            behaviour.Instance = behaviour.Instance;


            return behaviour;
        }

        public void InitializeProvider(IInstanceProvider instanceProvider)
        {
            Instance.InstanceProvider = instanceProvider;
        }
    }
}
