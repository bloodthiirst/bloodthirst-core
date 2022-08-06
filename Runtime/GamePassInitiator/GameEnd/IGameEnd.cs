using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.Setup
{
    public interface IGameEnd
    {
        int Order { get; }
        IAsynOperationWrapper GetAsyncOperations();
    }
}
