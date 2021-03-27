using Bloodthirst.Scripts.Core.UnityPool;
using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public abstract class EntityBehaviour<DATA, STATE, INSTANCE> : MonoBehaviour, IInitializeInstance, IHasEntityRegisterInstance, IInitializeProvider, IInitializeIdentifier,
        IBehaviour<INSTANCE>
        where DATA : EntityData
        where STATE : class, IEntityState<DATA>, new()
        where INSTANCE : EntityInstance<DATA, STATE, INSTANCE>, new()
    {

        /// <summary>
        /// The parent entity containing this behaviour
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private EntityIdentifier entityIdentifier;

        protected EntityIdentifier EntityIdentifier
        {
            get => entityIdentifier;
            set => entityIdentifier = value;
        }

        private IEntityInstanceRegister entityInstanceRegister;
        protected IEntityInstanceRegister EntityInstanceRegister
        {
            get => entityInstanceRegister;
            set => entityInstanceRegister = value;
        }

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
                    EntityInstanceRegister.Unregister(instance);
                    InstanceRegister<INSTANCE>.Unregister(instance);

                    instance.BeforeEntityRemoved -= OnEntityRemove;
                    instance.OnIsActiveChanged -= OnIsActiveChanged;

                    OnDisposedInstance(instance);
                    instance.NotifyInstanceDisposed();
                }

                instance = value;

                if (instance == null)
                    return;

                instance.EntityIdentifier = EntityIdentifier;

                EntityInstanceRegister.Register(instance);
                InstanceRegister<INSTANCE>.Register(instance);

                instance.BeforeEntityRemoved -= OnEntityRemove;
                instance.BeforeEntityRemoved += OnEntityRemove;
                instance.OnIsActiveChanged += OnIsActiveChanged;

                OnSetInstance(instance);
                instance.NotifyInstanceBinded();

                instance.NotifyStateChanged();
            }
        }

        protected virtual void OnIsActiveChanged(INSTANCE instance)
        {
            if (instance.IsActive)
            {
                EntityInstanceRegister.Register(instance);
                InstanceRegister<INSTANCE>.Register(instance);
            }
            else
            {
                EntityInstanceRegister.Unregister(instance);
                InstanceRegister<INSTANCE>.Unregister(instance);
            }

        }

        public DATA Data => instance.State.Data;

        public STATE State => instance.State;

        [SerializeField]
        private DATA tagData = default;

        public DATA TagData => tagData;


        [SerializeField]
        protected INSTANCE instance = default;

        private void OnValidate()
        {
            if (entityIdentifier == null)
            {
                entityIdentifier = GetComponent<EntityIdentifier>();
            }
        }

        /// <summary>
        /// <para>Inject the instance either by using the exterior data or by using the serialized instance</para>
        /// <para>This should only be called by the <see cref="EntitySpawner" and injection points/></para>
        /// </summary>
        protected virtual void InitializeInstance(EntityIdentifier entityIdentifier)
        {
            IInstanceInjector<INSTANCE> injector = GetComponentInChildren<IInstanceInjector<INSTANCE>>();

            if (injector != null)
            {
                Instance = injector.GetInstance();
                return;
            }

            Instance = Instance;
        }

        /// <summary>
        /// <para>Gets invoked after the instance  calls for a remove </para>
        /// <para>Apply the removal that needs to be done on the behaviour part, like invoking Destroy or sending back to the pool</para>
        /// </summary>
        /// <param name="ins"></param>
        public virtual void OnEntityRemove(INSTANCE ins)
        {
            Instance = null;
        }

        protected virtual void OnDestroy()
        {
            if (Instance == null)
                return;

            instance.BeforeEntityRemoved -= OnEntityRemove;
            instance = null;
        }

        /// <summary>
        /// <para>When the <see cref="Instance"/> is being changed , this method is invoked with the OLD instance passed as parameter</para>
        /// <para>Use this method to remove any links made to the old instance </para>
        /// </summary>
        /// <param name="instance"></param>
        public abstract void OnDisposedInstance(INSTANCE instance);

        /// <summary>
        /// Method invoked after the instance has been changed
        /// </summary>
        /// <param name="instance"></param>
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
            BEHAVIOUR behaviour = GenericUnityPool.Instance.Get<BEHAVIOUR>();

            behaviour.Instance.State = state;

            behaviour.Instance = behaviour.Instance;


            return behaviour;
        }
        void IInitializeInstance.InitializeInstance(EntityIdentifier entityIdentifier)
        {
            InitializeInstance(entityIdentifier);
        }

        void IHasEntityRegisterInstance.ProvideEntityInstanceInstance(IEntityInstanceRegister instanceRegister)
        {
            EntityInstanceRegister = instanceRegister;
        }

        void IInitializeProvider.InitializeProvider(IInstanceProvider instanceProvider)
        {
            Instance.InstanceProvider = instanceProvider;
        }

        void IInitializeIdentifier.InitializeIdentifier(EntityIdentifier entityIdentifier)
        {
            EntityIdentifier = entityIdentifier;
        }
    }
}
