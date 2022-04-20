using System.Collections;

namespace Bloodthirst.Core.AdvancedPool
{
    public class ListProcessor<TList> : IPoolProcessor<TList> where TList : IList, new()
    {
        void IPoolProcessor<TList>.BeforeGet(TList list)
        {
            
        }

        void IPoolProcessor<TList>.BeforeReturn(TList list)
        {
            list.Clear();
        }
    }
}