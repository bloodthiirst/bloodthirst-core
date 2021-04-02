using System.Collections.Generic;

namespace Bloodthirst.Core.TreeList
{
    public struct TreeLeafInfo<TKey, TElement> where TKey : class
    {
        public HashSet<TKey> KeysEncountered { get; set; }
        public TreeLeaf<TKey, TElement> TreeLeaf { get; set; }

        public int Depth { get; set; }

    }
}
