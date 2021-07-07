using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace Bloodthirst.System.Quest.Editor
{
    public class CustomContext
    {
        public List<UnityEngine.Object> UnityObjects { get; set; }
        public object Root { get; set; }
    }
}
