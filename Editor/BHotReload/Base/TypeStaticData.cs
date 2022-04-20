using System;
using System.Collections.Generic;

namespace Bloodthirst.Editor.BHotReload
{
    public struct TypeStaticData
    {
        public Type TypeReference { get; set; }
        public Dictionary<string,object> StaticFields { get; set; }
        public Dictionary<string,object> StaticProperties { get; set; }
    }
}
