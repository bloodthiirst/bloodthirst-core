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

    public abstract class EntityBehaviour<DATA, STATE, INSTANCE> : MonoBehaviour, IInitializeInstance, IRegisterInstance, IInitializeProvider, IInitializeIdentifier,
        IBehaviour<INSTANCE>
        where DATA : EntityData
        where STATE : class, IEntityState<DATA>, new()
        where INSTANCE : EntityInstance<DATA, STATE, INSTANCE>, new()
    {
        [SerializeField]
        private EntityIdentifier entityIdentifier;

        protected EntityIdentifier EntityIdentifier { get => entityIdentifier; set => entityIdentifier = value; }

        public INSTANCE Instance
        {
            get
            {
                return instance;
            }
            set
            {
                if (instance != null)
                {
                    InstanceRegister<INSTANCE>.Unregister(instance);
                    instance.BeforeEntityRemoved -= OnRemove;
                }

                instance = value;

                InstanceRegister<INSTANCE>.Register(instance);
                instance.BeforeEntityRemoved -= OnRemove;
                instance.BeforeEntityRemoved += OnRemove;

                OnSetInstance(instance);

                instance.NotifyStateChanged();
            }
        }

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
        protected INSTANCE instance = default;

        private void OnValidate()
        {
            if (entityIdentifier == null)
            {
                entityIdentifier = GetComponent<EntityIdentifier>();
            }
        }

        /// <summary>
        /// Inject the instance either by using the exterior data or by using the serialized instance
        /// </summary>
        public virtual void InitializeInstance(EntityIdentifier entityIdentifier)
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

        public virtual void OnRemove(INSTANCE ins)
        {
            // TODO : do the actual behaviour , like invoking Destroy or sending back to the pool
            instance.BeforeEntityRemoved -= OnRemove;
            instance = null;
        }

        protected virtual void OnDestroy()
        {
            if (Instance == null)
                return;

            instance.BeforeEntityRemoved -= OnRemove;
            instance = null;
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

        public void InitializeIdentifier(EntityIdentifier entityIdentifier)
        {
            EntityIdentifier = entityIdentifier;

            Instance.EntityIdentifier = entityIdentifier;
        }
    }
}
