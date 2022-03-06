using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.Setup
{
        public class AsyncOperationGroup
        {
            public int Count { get; private set; }
            public bool IsDone { get; private set; }
            public float Progress { get; private set; }

            private List<AsyncOperation> AsyncOperations { get; set; }

            public AsyncOperationGroup(IEnumerable<AsyncOperation> asyncOps)
            {
                AsyncOperations = new List<AsyncOperation>(asyncOps);
            }
            public void Add(AsyncOperation asyncOperation)
            {
                AsyncOperations.Add(asyncOperation);
            }

            public bool Remove(AsyncOperation asyncOperation)
            {
                return AsyncOperations.Remove(asyncOperation);
            }

            public void Refresh()
            {
                float currentProg = 0;
                Count = AsyncOperations.Count;

                for (int i = 0; i < Count; i++)
                {
                    AsyncOperation curr = AsyncOperations[i];

                    if (curr.isDone)
                    {
                        currentProg += 1;
                        continue;
                    }

                    currentProg += curr.progress;
                }

                Progress = currentProg / Count;
                IsDone = currentProg == Count;
            }
        }
}