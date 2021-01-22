using System.Collections.Generic;

namespace JsonDB
{
    public class DbEntityComparer<T> : IEqualityComparer<T> where T : IDbEntity
    {
        public bool Equals(T x, T y)
        {
            if (x == null)
                return false;

            if (y == null)
                return false;

            return x.EntityId == y.EntityId;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
