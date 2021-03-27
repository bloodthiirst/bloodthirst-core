using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    /// <summary>
    /// <para>This component is the main identifying part of the game entities</para>
    /// <para>It is meant to be the main primary compnent that has multiple BISD instances under it to form a complete game entity</para>
    /// <para>It acts as the "parent" of the entity and it's main identitfier , it also determines the lifecycle of the entity since it is used to spawn and remove the entity from the game world</para>
    /// </summary>
    public class EntityIdentifier : MonoBehaviour
    {
        [SerializeField]
        private int id;

        public int Id
        {
            get => id;
            set
            {
                id = value;
                ChangeEntityID(id);
            }
        }

        private void ChangeEntityID(int identifier)
        {
            IEntityInstance[] all = GetComponentsInChildren<IEntityInstance>(true);

            for (int i = 0; i < all.Length; i++)
            {
                IEntityInstance curr = all[i];

                curr.State.Id = id;
            }
        }


        /// <summary>
        /// <para>Event triggered when an entity is spawned</para>
        /// <para>This event invoked after all the injection steps have taken place</para>
        /// </summary>
        public event Action<EntityIdentifier> OnEntitySpawned;

        /// <summary>
        /// <para>Event triggered when an entity is being removed</para>
        /// <para>This could mean either sent back to a pool or destroyed , depending on the spawners behaviour</para>
        /// </summary>
        public event Action<EntityIdentifier> OnEntityRemoved;

        [SerializeField]
        private PrefabReferenceData prefabReferenceData;

        /// <summary>
        /// Prefab used to spawn this entity
        /// </summary>
        public PrefabReferenceData PrefabReferenceData => prefabReferenceData;

        [SerializeField]
        private EntityType entityType;

        /// <summary>
        /// <para>Serve as the "TYPE ID" for the game entities</para>
        /// </summary>
        public EntityType EntityType => entityType;

        [Button]
        /// <summary>
        /// <para>Announce the removal of the entity to the concerned objects</para>
        /// <para>Used by the spawners to control the lifecycle of the game entity</para>
        /// </summary>
        public void TriggerRemoved()
        {
            OnEntityRemoved?.Invoke(this);
        }

        [Button]
        /// <summary>
        /// <para>Announce the spawn of the entity to the concerned objects</para>
        /// <para>Used by the spawners to control the lifecycle of the game entity</para>
        /// </summary>
        public void TriggerSpawned()
        {
            OnEntitySpawned?.Invoke(this);
        }

    }
}
