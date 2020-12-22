using Newtonsoft.Json;
using System;

namespace JsonDB.CustomJsonConverter
{
    public class IDbReferenceConverter : JsonConverter<IDbReference>
    {
        public override IDbReference ReadJson(JsonReader reader, Type objectType, IDbReference existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            IDbReference casted = (IDbReference)reader.Value;
            existingValue.ReferenceId = casted.ReferenceId;

            return casted;
        }

        public override void WriteJson(JsonWriter writer, IDbReference value, JsonSerializer serializer)
        {
            if (value == null || !value.ReferenceId.HasValue)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value.ReferenceId.Value);
            }

        }
    }
}
