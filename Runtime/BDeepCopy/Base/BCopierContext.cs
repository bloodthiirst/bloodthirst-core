using System.Collections.Generic;

namespace Bloodthirst.BDeepCopy
{
    public class BCopierContext
    {
        private Dictionary<object, object> OriginalsToCopies { get; set; } = new Dictionary<object, object>();

        public bool TryGetCached(object original , out object cached)
        {
            return OriginalsToCopies.TryGetValue(original, out cached);
        }

        public void Register(object original , object cached)
        {
            OriginalsToCopies.Add(original, cached);
        }
        
    }
}
