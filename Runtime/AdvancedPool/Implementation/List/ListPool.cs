using System.Collections;
using System.Collections.Generic;

namespace Bloodthirst.Core.AdvancedPool
{
    public class ListPool<TList> : GenericPool<TList, ListProcessor<TList>> where TList : IList, new()
    {

    }
}