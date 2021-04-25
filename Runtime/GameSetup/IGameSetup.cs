using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.Setup
{
    public interface IGameSetup
    {
        int Order { get; }
        IEnumerable<AsyncOperation> Operations(); 
    }
}
