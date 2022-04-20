using Bloodthirst.Core.AdvancedPool.Pools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IEntitySpawner
    {
        BEHAVIOUR AddBehaviour<BEHAVIOUR, INSTANCE, STATE, DATA>(MonoBehaviour entity)
            where BEHAVIOUR : EntityBehaviour<DATA, STATE, INSTANCE>
            where INSTANCE : EntityInstance<DATA, STATE, INSTANCE>, new()
            where STATE : class, IEntityState<DATA>, new()
            where DATA : EntityData;
        IGlobalPool GenericUnityPool { get; set; }
        T InjectStates<T>(T entity) where T : MonoBehaviour;
        List<GameObject> LoadGameState(GameStateSaveInstance gameData);
        void RemoveEntity<T>(T player) where T : MonoBehaviour;
        T SpawnEntity<T>() where T : MonoBehaviour;
        T SpawnEntity<T>(Predicate<T> filter) where T : MonoBehaviour;
    }
}