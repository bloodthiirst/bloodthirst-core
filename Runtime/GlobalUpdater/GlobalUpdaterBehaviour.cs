using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Core.Updater
{
    public class GlobalUpdaterBehaviour : MonoBehaviour, IGlobalUpdater
    {
        private List<IUpdater> updaters;

        public void Initialize()
        {
            updaters = new List<IUpdater>();
        }

        void IGlobalUpdater.Register(IUpdater updater)
        {
            Assert.IsFalse(updaters.Contains(updater));

            updaters.Add(updater);
        }

        void IGlobalUpdater.Unregister(IUpdater updater)
        {
            Assert.IsTrue(updaters.Contains(updater));

            updaters.Remove(updater);
        }

        private void Update()
        {
            for (int i = 0; i < updaters.Count; ++i)
            {
                updaters[i].Tick(Time.deltaTime);
            }
        }
    }
}
