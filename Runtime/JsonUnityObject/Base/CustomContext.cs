using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace Bloodthirst.JsonUnityObject
{
    public struct CustomContext
    {
        public List<UnityEngine.Object> UnityObjects { get; set; }
    }
}
