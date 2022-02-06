using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bloodthirst.JsonUnityObject
{
    public class UnityObjectContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            PropertyInfo dict = type.GetProperty("dict", BindingFlags.NonPublic | BindingFlags.Instance);

            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            if (dict != null)
            {
                JsonProperty jsonProp = CreateProperty(dict, memberSerialization);
                jsonProp.Ignored = false;
                jsonProp.TypeNameHandling = TypeNameHandling.All;
                jsonProp.Writable = true;
                jsonProp.Readable = true;
                properties.Add(jsonProp);
            }

            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                foreach (JsonProperty property in properties)
                {
                    if ( JsonUnityObjectSettings.MonoBehaviourIgnorableMembers.Contains(property.PropertyName))
                        property.Ignored = true;

                }
            }

            return properties;
        }
    }
}
