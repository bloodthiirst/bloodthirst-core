using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Bloodthirst.JsonUnityObject
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WriteEndObject();
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            object t = serializer.Deserialize(reader);
            Vector2 iv = JsonConvert.DeserializeObject<Vector2>(t.ToString());
            return iv;
        }
    }
}
