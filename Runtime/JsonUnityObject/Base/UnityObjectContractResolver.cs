using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;


namespace Bloodthirst.JsonUnityObject
{
    public class UnityObjectContractResolver : DefaultContractResolver
    {
        private const string HIDE_FLAG_FIELD = "hideFlags";
        private const string NAME_FIELD = "name";

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                foreach (JsonProperty property in properties)
                {
                    if (property.PropertyName.Equals(NAME_FIELD) || property.PropertyName.Equals(HIDE_FLAG_FIELD))
                        property.Ignored = true;

                }
            }

            return properties;
        }
    }
}
