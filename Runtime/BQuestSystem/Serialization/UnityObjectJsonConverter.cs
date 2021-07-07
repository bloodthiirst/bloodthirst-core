using Bloodthirst.Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bloodthirst.System.Quest.Editor
{
    public partial class BQuestSystemSettings
    {
        public class UnityObjectJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {

                CustomContext ctx = serializer.Context.Context as CustomContext;

                if (value == ctx.Root)
                {
                    Type t = value.GetType();

                    List<MemberInfo> mems = new List<MemberInfo>();

                    writer.WriteStartObject();

                    foreach (MemberInfo m in t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (m.Name.Equals("name") || m.Name.Equals("hideFlags"))
                            continue;

                        if (m.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                            continue;

                        switch (m)
                        {
                            case FieldInfo f:
                                {
                                    if (f.Name.EndsWith("k__BackingField"))
                                        continue;

                                    mems.Add(f);
                                    var val = f.GetValue(value);
                                    writer.WritePropertyName(f.Name);
                                    serializer.Serialize(writer, val);
                                    break;
                                }
                            case PropertyInfo p:
                                {
                                    mems.Add(p);
                                    var val = p.GetValue(value);
                                    writer.WritePropertyName(p.Name);
                                    serializer.Serialize(writer, val);

                                    break;
                                }
                            default:
                                break;
                        }
                    }

                    writer.WriteEndObject();

                    return;
                }

                if (value == null)
                {
                    writer.WriteNull();
                    return;
                }

                UnityEngine.Object asset = (UnityEngine.Object)value;

                // save asset and get the index
                int id = ctx.UnityObjects.Count;
                ctx.UnityObjects.Add(asset);

                /*
                string path = AssetDatabase.GetAssetPath(asset);

                writer.WriteValue(path);
                */
                writer.WriteValue(id);

            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {

                if (reader.Value == null)
                {
                    return null;
                }

                long id = (long)reader.Value;

                CustomContext ctx = serializer.Context.Context as CustomContext;


                UnityEngine.Object asset = ctx.UnityObjects[(int)id];

                return asset;
            }

            public override bool CanConvert(Type objectType)
            {
                if (!TypeUtils.IsSubTypeOf(objectType, typeof(UnityEngine.Object)))
                    return false;

                // ok , this is a UnityEngine.Object instance
                return true;
                //return objectType != typeof(NodeTreeData);
            }
        }

    }
}
