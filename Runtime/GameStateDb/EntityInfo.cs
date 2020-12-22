using System;
using System.Collections.Generic;

namespace JsonDB
{
    public class EntityInfo
    {
        public Type EntityType { get; set; }
        public List<SingleReferenceProperty> SingleReferenceInjectors { get; set; }
        public List<MultipleReferenceProperty> MultipleReferenceInjectors { get; set; }
    }
}
