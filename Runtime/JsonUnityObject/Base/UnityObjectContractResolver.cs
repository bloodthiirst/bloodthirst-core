using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.JsonUnityObject
{
    public class UnityObjectContractResolver : DefaultContractResolver
    {
        private const string HIDE_FLAG_FIELD = "hideFlags";
        private const string NAME_FIELD = "name";
        private const string GAMEOBJECT_FIELD = "gameObject";

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

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
