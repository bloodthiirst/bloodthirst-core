using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Core.Updater
{
    public class DefaultUpdaterBehaviour : MonoBehaviour, IUpdater
    {
        private List<IUpdatable> updatables = new List<IUpdatable>();

        void IUpdater.Register(IUpdatable updatable)
        {
            Assert.IsFalse(updatables.Contains(updatable));

            updatables.Add(updatable);
        }

        void IUpdater.Unregister(IUpdatable updatable)
        {
            Assert.IsTrue(updatables.Contains(updatable));

            updatables.Remove(updatable);
        }

        void IUpdater.Tick(float deltaTime)
        {
            for (int i = 0; i < updatables.Count; ++i)
            {
                updatables[i].OnTick(deltaTime);
            }
        }
    }
}
