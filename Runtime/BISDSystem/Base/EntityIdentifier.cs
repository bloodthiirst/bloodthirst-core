using Bloodthirst.Core.AdvancedPool;
using Bloodthirst.Core.AdvancedPool.Pools;
using Bloodthirst.Core.BProvider;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace Bloodthirst.Core.BISDSystem
{

    /// <summary>
    /// <para>This component is the main identifying part of the game entities</para>
    /// <para>It is meant to be the main primary compnent that has multiple BISD instances under it to form a complete game entity</para>
    /// <para>It acts as the "parent" of the entity and it's main identitfier , it also determines the lifecycle of the entity since it is used to spawn and remove the entity from the game world</para>
    /// </summary>
    [GeneratePool]
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
                HandleIdChanged(id);
            }
        }

        private void HandleIdChanged(int identifier)
        {
            using (ListPool<IBehaviourInstance>.Get(out List<IBehaviourInstance> all))
            {
                GetComponentsInChildren(true, all);

                for (int i = 0; i < all.Count; i++)
                {
                    IBehaviourInstance curr = all[i];

                    if(curr.Instance?.State == null)
                    {
                        continue;
                    }

                    IEntityState state = curr.Instance.State;

                    state.Id = identifier;

                    curr.Instance.State = state;
                }
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

#if ODIN_INSPECTOR
        [Button]
#endif

        /// <summary>
        /// <para>Announce the removal of the entity to the concerned objects</para>
        /// <para>Used by the spawners to control the lifecycle of the game entity</para>
        /// </summary>
        public void TriggerRemoved()
        {
            OnEntityRemoved?.Invoke(this);
        }


#if ODIN_INSPECTOR
        [Button]
#endif

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
