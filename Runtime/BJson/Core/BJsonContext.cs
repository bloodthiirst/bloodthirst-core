using System.Collections.Generic;

namespace Bloodthirst.BJson
{
    public class BJsonContext
    {
        public int Indentation { get; set; }
        private Dictionary<int, object> IdToInstance { get; set; } = new Dictionary<int, object>();

        internal int CachingCount => IdToInstance.Count;

        public bool TryGetCached(int id , out object cached)
        {
            return IdToInstance.TryGetValue(id, out cached);
        }

        public int Register(object original)
        {
            int id = IdToInstance.Count;
            IdToInstance.Add(id, original);

            return id;
        }
        
        public bool IsCached(object instance , out int id)
        {
            foreach(KeyValuePair<int, object> kv in IdToInstance)
            { 
                if(kv.Value == instance)
                {
                    id = kv.Key;
                    return true;
                }
            }

            id = -1;
            return false;
        }
    }
}
