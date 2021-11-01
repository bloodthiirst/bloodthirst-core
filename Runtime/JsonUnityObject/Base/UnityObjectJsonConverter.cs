using Bloodthirst.Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bloodthirst.JsonUnityObject
{

    public class UnityObjectJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            CustomContext ctx = serializer.Context.Context as CustomContext;

            // if we are at the root of the object
            // that means that we need to draw as a normal json body and not with the ID
            //
            // unfortunaly we have to do this manually
            // this part could be optimized by caching the getters/setter per type
            // of generating getters/setters as lambdas and caching them
            // old check => if (value == ctx.Root)
            if (writer.Path == string.Empty)
            {
                Type t = value.GetType();

                List<MemberInfo> mems = new List<MemberInfo>();

                // start the object 
                // { 
                writer.WriteStartObject();

                foreach (MemberInfo m in t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    // we have to redo the check here since we are doing everything manually
                    
                    // so we ignore these fields since unity doesn't want us touching them during serialization
                    if (m.Name.Equals("name") || m.Name.Equals("hideFlags"))
                        continue;

                    // we skip ignoratble members manually too
                    if (m.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                        continue;


                    switch (m)
                    {
                        case FieldInfo f:
                            {
                                // to avoid changign the internal field of an auto property
                                if (f.Name.EndsWith("k__BackingField"))
                                    continue;

                                mems.Add(f);
                                var val = f.GetValue(value);

                                // write the field name
                                // "StringFieldExample" :
                                writer.WritePropertyName(f.Name);

                                // write the actual value
                                // "StringFieldValue"
                                serializer.Serialize(writer, val);
                                break;
                            }
                        case PropertyInfo p:
                            {
                                mems.Add(p);
                                var val = p.GetValue(value);

                                // write the field name
                                // "StringFieldExample" :
                                writer.WritePropertyName(p.Name);

                                // write the actual value
                                // "StringFieldValue"
                                serializer.Serialize(writer, val);

                                break;
                            }
                        default:
                            break;
                    }
                }

                // close the object
                // }
                writer.WriteEndObject();

                // we're all done here in the case of the root object
                return;
            }

            // if null
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // if we get to this part
            // that means that we have hit an object that is a sub-object and NOT the root
            // so we use the index  thingy
            UnityEngine.Object asset = (UnityEngine.Object)value;

            // get the id of the object
            int id = ctx.UnityObjects.Count;

            // add the object
            ctx.UnityObjects.Add(asset);

            // save the id
            writer.WriteValue(id);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //if the value in the json is null
            // then we simple just return null
            if (reader.Value == null)
            {
                return null;
            }

            // if not then we get the value (the object's index) and cast it
            // i know this look weird since we saved it as int when we write
            // but we cast it as long when we read
            // which kinda makes sense , i guess the serializer assumes the worst case scenario (memory wise) and let's u handle the specific type
            long id = (long)reader.Value;

            // get the list from the context
            CustomContext ctx = serializer.Context.Context as CustomContext;

            // and fetch the unity object using the index
            UnityEngine.Object asset = ctx.UnityObjects[(int)id];

            return asset;
        }

        public override bool CanConvert(Type objectType)
        {
            if (!TypeUtils.IsSubTypeOf(objectType, typeof(UnityEngine.Object)))
                return false;

            return true;
        }
    }
}
