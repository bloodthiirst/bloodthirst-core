using System.Collections.Generic;

namespace Bloodthirst.Core.AdvancedPool
{
    public class StaticPool<TObject> where TObject : class, IOnSpawn , IOnDespawn , new()
    {
        private static StaticPool<TObject> instance;
        private static StaticPool<TObject> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StaticPool<TObject>();
                }
                return instance;
            }
        }

        public static int PoolSpawnSize { get; set; } = 1000;

        private Queue<TObject> poolQueue;
        private StaticPool()
        {
            poolQueue = new Queue<TObject>();
        }

        private void CheckPoolSize()
        {
            if(poolQueue.Count == 0)
            {
                for(int i = 0; i < PoolSpawnSize; i++)
                {
                    poolQueue.Enqueue(new TObject());
                }
            }
        }

        public static TObject Get()
        {
            Instance.CheckPoolSize();

            TObject ins = Instance.poolQueue.Dequeue();
            ins.OnSpawn();

            return ins;
        }

        public static void Return(TObject obj)
        {
            obj.OnDespawn();

            Instance.poolQueue.Enqueue(obj);
        }

    }
}