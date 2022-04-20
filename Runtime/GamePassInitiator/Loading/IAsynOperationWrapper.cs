using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.Setup
{
    public interface IAsynOperationWrapper
    {
        int Order { get; }

        int OperationsCount();
        IEnumerable<AsyncOperation> StartOperations();
    }
}
