using System.Collections.Generic;

namespace Bloodthirst.Core.Setup
{
    public interface IGameSetup
    {
        IEnumerable<IAsynOperationWrapper> GetAsynOperations();
    }
}
