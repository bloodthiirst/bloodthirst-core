using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace JsonDB
{
    public class MultipleReferenceProperty
    {
        public Type ReferenceType { get; set; }
        public PropertyInfo PropertyAccessor { get; set; }
        public Func<IDbReference> ReferenceCreator { get; set; }
    }
}
