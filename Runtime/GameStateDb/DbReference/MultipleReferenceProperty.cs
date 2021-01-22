using System;
using System.Reflection;

namespace JsonDB
{
    public class MultipleReferenceProperty
    {
        public Type ReferenceType { get; set; }
        public PropertyInfo PropertyAccessor { get; set; }
        public Func<IDbReference> ReferenceCreator { get; set; }
    }
}
