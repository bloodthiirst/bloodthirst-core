using Bloodthirst.Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Bloodthirst.System.Quest.Editor
{
    public partial class BQuestSystemSettings
    {
        public class UnityObjectJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if(value == null)
                {
                    writer.WriteNull();
                    return;
                }

                List<UnityEngine.Object> ctx = serializer.Context.Context as List<UnityEngine.Object>;

                UnityEngine.Object asset = (UnityEngine.Object)value;
                
                // save asset and get the index
                int id = ctx.Count;
                ctx.Add(asset);

                /*
                string path = AssetDatabase.GetAssetPath(asset);

                writer.WriteValue(path);
                */
                writer.WriteValue(id);

            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {            
            
                if(reader.Value == null)
                {
                    return null;
                }

                long id = (long)reader.Value;

                List<UnityEngine.Object> ctx = serializer.Context.Context as List<UnityEngine.Object>;


                UnityEngine.Object asset = ctx[(int)id]; 

                return asset;
            }

            public override bool CanConvert(Type objectType)
            {
                if (!TypeUtils.IsSubTypeOf(objectType, typeof(UnityEngine.Object)))
                    return false;

                // ok , this is a UnityEngine.Object instance

                return objectType != typeof(NodeTreeData);
            }
        }

    }
}
