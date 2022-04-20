using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.BJson
{
    public class UnityObjectContext
    {
        public List<UnityEngine.Object> UnityObjects { get; set; }

        public Dictionary<string, GameObject[]> allSceneObjects { get; set; }
    }
}
