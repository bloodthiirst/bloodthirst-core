using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Bloodthirst.System.Quest.Editor
{
    public partial class BQuestSystemSettings
    {
        public class UnityObjectContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {

                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
                if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    foreach (JsonProperty property in properties)
                    {
                        if (property.PropertyName.Equals("name") || property.PropertyName.Equals("hideFlags"))
                            property.Ignored = true;

                    }
                }
                return properties;
            }
        }

    }
}
