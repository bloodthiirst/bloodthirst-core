using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public abstract class EntityBehaviour<DATA, STATE, INSTANCE> : MonoBehaviour,
        IInitializeIdentifier,
        IInitializeInstance,
        IHasEntityInstanceRegister,
        IHasEntityInstanceProvider,
        IBehaviour,
        IPostEntityLoaded
        where DATA : EntityData
        where STATE : class, IEntityState<DATA>, new()
        where INSTANCE : EntityInstance<DATA, STATE, INSTANCE>, new()
    {

        private readonly Type stateType = typeof(STATE);

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

        public DATA Data => instance.State.Data;

        public STATE State => instance.State;

        [SerializeField]
        private DATA tagData = default;

        public DATA TagData => tagData;

        [SerializeField]
        protected INSTANCE instance = default;

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
                    EntityInstanceRegister?.Unregister(instance);
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

                EntityInstanceRegister?.Register(instance);
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

        #region Unity callbacks
        protected virtual void OnValidate()
        {
            if (entityIdentifier == null)
            {
                entityIdentifier = GetComponent<EntityIdentifier>();
            }
        }
        protected virtual void OnDestroy()
        {
            if (Instance == null)
                return;

            instance.BeforeEntityRemoved -= OnEntityRemove;
            instance = null;
        }
        #endregion

        /// <summary>
        /// <para>Inject the instance either by using the exterior data or by using the serialized instance</para>
        /// <para>This should only be called by the <see cref="EntitySpawner" and injection points/></para>
        /// </summary>
        protected virtual void InitializeInstance(EntityIdentifier entityIdentifier, IEntityState preloadState)
        {
            if (preloadState != null)
            {
                STATE state = (STATE)preloadState;
                state.PreloadStateFromData();

                INSTANCE loaded = new INSTANCE
                {
                    State = state
                };

                Instance = loaded;
                return;
            }

            IInstanceInjector<INSTANCE> injector = GetComponentInChildren<IInstanceInjector<INSTANCE>>();

            if (injector != null)
            {
                Instance = injector.GetInstance();
                return;
            }

            // in case state isn't serializable
            if (Instance.State == null)
            {
                STATE state = new STATE();
                state.Data = TagData;
                state.PreloadStateFromData();

                INSTANCE defaultInstance = new INSTANCE
                {
                    State = state
                };

                Instance = defaultInstance;
                return;
            }

            Instance.State.PreloadStateFromData();
            Instance = Instance;
        }

        /// <summary>
        /// Method invoked after the instance has been changed
        /// </summary>
        /// <param name="instance"></param>
        public abstract void OnSetInstance(INSTANCE instance);

        /// <summary>
        /// <para>When the <see cref="Instance"/> is being changed , this method is invoked with the OLD instance passed as parameter</para>
        /// <para>Use this method to remove any links made to the old instance </para>
        /// </summary>
        /// <param name="instance"></param>
        public abstract void OnDisposedInstance(INSTANCE instance);

        /// <summary>
        /// <para>Gets invoked after the instance  calls for a remove </para>
        /// <para>Apply the removal that needs to be done on the behaviour part, like invoking Destroy or sending back to the pool</para>
        /// </summary>
        /// <param name="ins"></param>
        public virtual void OnEntityRemove(INSTANCE ins)
        {
            Instance = null;
        }

        /// <summary>
        /// <para>Method called by the entity spawner after all loading is done</para>
        /// <para>Usefull when we wanna look for a specific entity but we need to make sure that all the data is present in the scene</para>
        /// </summary>
        protected virtual void PostEntityLoaded() { }

        #region interface implementations
        IEntityInstance IBehaviour.Instance => Instance;

        Type IInitializeInstance.StateType => stateType;

        void IInitializeInstance.InitializeInstance(EntityIdentifier entityIdentifier, IEntityState preloadState)
        {
            InitializeInstance(entityIdentifier, preloadState);
        }

        void IHasEntityInstanceRegister.InitializeEntityInstanceRegister(IEntityInstanceRegister instanceRegister)
        {
            EntityInstanceRegister = instanceRegister;
        }

        void IHasEntityInstanceProvider.InitializeEntityInstanceProvider(IInstanceProvider instanceProvider)
        {
            Instance.InstanceProvider = instanceProvider;
        }

        void IInitializeIdentifier.InitializeIdentifier(EntityIdentifier entityIdentifier)
        {
            EntityIdentifier = entityIdentifier;
        }

        void IPostEntityLoaded.PostEntityLoaded()
        {
            PostEntityLoaded();
        }
        #endregion
    }
}
