using System.Collections;
using System.Collections.Generic;

namespace Bloodthirst.Core.AdvancedPool
{
    public class GenericPool<TObject, TProcessor>
        where TObject : new()
        where TProcessor : IPoolProcessor<TObject> , new()
    {
        public static int PoolSpawnSize { get; set; } = 1000;

        private static TProcessor processor;

        private static GenericPool<TObject , TProcessor> instance;
        private static GenericPool<TObject , TProcessor> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GenericPool<TObject , TProcessor>();
                }
                return instance;
            }
        }

        private Queue<TObject> poolQueue;
        protected GenericPool()
        {
            poolQueue = new Queue<TObject>();
            processor = new TProcessor();
        }

        private void CheckPoolSize()
        {
            if (poolQueue.Count == 0)
            {
                for (int i = 0; i < PoolSpawnSize; i++)
                {
                    poolQueue.Enqueue(new TObject());
                }
            }
        }

        public static TObject Get()
        {
            Instance.CheckPoolSize();

            TObject ins = Instance.poolQueue.Dequeue();

            processor.BeforeGet(ins);

            return ins;
        }

        public static void Return(TObject obj)
        {
            processor.BeforeReturn(obj);

            Instance.poolQueue.Enqueue(obj);
        }

    }

    public class GenericPool<TObject> where TObject : new()
    {
        public static int PoolSpawnSize { get; set; } = 1000;       

        private static GenericPool<TObject> instance;
        private static GenericPool<TObject> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GenericPool<TObject>();
                }
                return instance;
            }
        }

        private Queue<TObject> poolQueue;
        private GenericPool()
        {
            poolQueue = new Queue<TObject>();
        }

        private void CheckPoolSize()
        {
            if (poolQueue.Count == 0)
            {
                for (int i = 0; i < PoolSpawnSize; i++)
                {
                    poolQueue.Enqueue(new TObject());
                }
            }
        }

        public static TObject Get()
        {
            Instance.CheckPoolSize();

            TObject ins = Instance.poolQueue.Dequeue();

            return ins;
        }

        public static void Return(TObject obj)
        {
            Instance.poolQueue.Enqueue(obj);
        }

    }
}