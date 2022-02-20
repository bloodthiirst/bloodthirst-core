using Bloodthirst.BJson;
using Bloodthirst.Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnityEngine;

namespace Bloodthirst.JsonUnityObject
{
    public static class JsonUnityObjectUtils
    {
        /// <summary>
        /// Utility method to deserialize the JSON data into the unity object
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="unityObjectThis"></param>
        /// <param name="unityRefsList"></param>
        public static void DeserializeUnityObject(string jsonString, UnityEngine.Object unityObjectThis, List<UnityEngine.Object> unityRefsList)
        {
            // get pooled settings
            BJsonSettings settings = JsonUnityObjectSettings.GetSettings();

            // custom context
            // make it a struct to minimize GC
            settings.CustomContext = new UnityObjectContext() { UnityObjects = unityRefsList };

            // deserialize into the object
            BJsonConverter.PopulateFromJson(unityObjectThis, jsonString , settings);
        }

        /// <summary>
        /// Utility method to serialize the unity object into valid JSON while keeping the unity object refs
        /// </summary>
        /// <returns></returns>
        public static string SerializeUnityObject(UnityEngine.Object unityObjectThis, List<UnityEngine.Object> unityRefsList)
        {
            // init/clear the list of unity object references
            unityRefsList = unityRefsList.CreateOrClear();

            // get pooled settings
            BJsonSettings settings = JsonUnityObjectSettings.GetSettings();

            // custom context
            // make it a struct to minimize GC
            settings.CustomContext = new UnityObjectContext() { UnityObjects = unityRefsList };

            // serialize the object into JSON
            string res = BJsonConverter.ToJson(unityObjectThis, unityObjectThis.GetType(), settings);

            return res;


        }
    }
}
