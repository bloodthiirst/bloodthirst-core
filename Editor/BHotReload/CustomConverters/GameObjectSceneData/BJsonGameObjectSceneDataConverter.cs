using Bloodthirst.BType;
using Bloodthirst.Editor.BHotReload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodthirst.BJson
{
    public class BJsonGameObjectSceneDataConverter : BJsonComplexBaseConverter
    {
        private BTypeData TypeData { get; set; }

        private Dictionary<string, BMemberData> MemberToDataDictionary { get; set; }
        private IBJsonConverterInternal IntConverter { get; set; }
        private IBJsonConverterInternal TypeConverter { get; set; }
        private int MemberCount { get; set; }

        public BJsonGameObjectSceneDataConverter(Type t) : base(t)
        {

        }

        public override void Initialize()
        {
            // filter props
            TypeData = BJsonPropertyFilterProvider.GetFilteredProperties(ConvertType);

            MemberToDataDictionary = TypeData.MemberDatas.ToDictionary(k => k.MemberInfo.Name, k => k);
            IntConverter = Provider.GetConverter(typeof(int));
            TypeConverter = Provider.GetConverter(typeof(Type));
            MemberCount = MemberToDataDictionary.Count;
        }
        public override object CreateInstance_Internal()
        {
            return new GameObjectSceneData();
        }

        public override void Serialize_NonFormatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (instance == null)
            {
                jsonBuilder.Append(BJsonUtils.NULL_IDENTIFIER);
                return;
            }

            if (BJsonUtils.WriteIdNonFormatted(instance, jsonBuilder, context, settings))
            {
                return;
            }

            context.Register(instance);

            jsonBuilder.Append('{');

            BJsonUtils.WriteTypeInfoNonFormatted(instance, jsonBuilder, context, settings);

            foreach (KeyValuePair<string, BMemberData> kv in MemberToDataDictionary)
            {
                jsonBuilder.Append(kv.Key);

                jsonBuilder.Append(':');

                IBJsonConverterInternal cnv = null;

                if (!settings.HasCustomConverter(kv.Value.Type, out cnv))
                {
                    cnv = Provider.GetConverter(kv.Value.Type);
                }
                object val = kv.Value.MemberGetter(instance);
                cnv.Serialize_NonFormatted_Internal(val, jsonBuilder, context, settings);

                jsonBuilder.Append(',');
            }

            jsonBuilder[jsonBuilder.Length - 1] = '}';
        }

        public override void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            BJsonUtils.NewLineAtStart(jsonBuilder);

            jsonBuilder.AddIndentation(context.Indentation);

            if (instance == null)
            {
                jsonBuilder.Append(BJsonUtils.NULL_IDENTIFIER);
                return;
            }

            if (BJsonUtils.WriteIdFormatted(instance, jsonBuilder, context, settings))
            {
                return;
            }

            context.Register(instance);

            jsonBuilder.Append('{');

            jsonBuilder.Append(Environment.NewLine);

            context.Indentation++;

            BJsonUtils.WriteTypeInfoFormatted(instance, jsonBuilder, context, settings);
            jsonBuilder.Append(Environment.NewLine);

            foreach (KeyValuePair<string, BMemberData> kv in MemberToDataDictionary)
            {
                jsonBuilder.AddIndentation(context.Indentation);
                jsonBuilder.Append(kv.Key);
                jsonBuilder.Append(" : ");


                IBJsonConverterInternal cnv = null;

                object val = kv.Value.MemberGetter(instance);
                Type valType = val.GetType();

                if (!settings.HasCustomConverter(valType, out cnv))
                {
                    cnv = Provider.GetConverter(valType);
                }
                cnv.Serialize_Formatted_Internal(val, jsonBuilder, context, settings);

                jsonBuilder.Append(',');
                jsonBuilder.Append(Environment.NewLine);

            }

            context.Indentation--;

            // remove the extra last ','
            // we do -3 because "NewLine" is actually two characters \r\n
            jsonBuilder.Remove(jsonBuilder.Length - 3, 1);

            jsonBuilder.AddIndentation(context.Indentation);

            jsonBuilder.Append('}');
        }

        public override object Deserialize_Internal(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            if (BJsonUtils.IsCachedOrNull(ref parseState, context, settings, out object cached))
            {
                return cached;
            }

            object instance = CreateInstance_Internal();

            context.Register(instance);

            int startTokenIndex = parseState.CurrentTokenIndex;

            // skip the first object start
            parseState.CurrentTokenIndex++;

            // prepare
            GameObjectSceneData data = new GameObjectSceneData();
            int componentStartTokenIndex = -1;


            while (parseState.CurrentTokenIndex < parseState.Tokens.Count || componentStartTokenIndex != -1)
            {
                Token<JSONTokenType> currentToken = parseState.Tokens[parseState.CurrentTokenIndex];

                // skip spaces
                if (currentToken.TokenType == JSONTokenType.SPACE)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // exit if object ended
                if (currentToken.TokenType == JSONTokenType.OBJECT_END)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // stp while no identitifer
                if (currentToken.TokenType != JSONTokenType.IDENTIFIER)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // key found
                string key = currentToken.ToString();

                // skip until the first colon
                parseState.SkipUntil(ParserUtils.IsColon);

                parseState.CurrentTokenIndex++;
                currentToken = parseState.Tokens[parseState.CurrentTokenIndex];

                // skip space
                parseState.SkipWhile(ParserUtils.IsSkippableSpace);

                if (MemberToDataDictionary.TryGetValue(key, out BMemberData memData))
                {
                    switch (key)
                    {
                        case nameof(GameObjectSceneData.ScenePath):
                            {
                                IBJsonConverterInternal c = Provider.GetConverter(memData.Type);

                                object newVal = c.Deserialize_Internal(ref parseState, context, settings);

                                data.ScenePath = (string)newVal;
                                break;
                            }
                        case nameof(GameObjectSceneData.GameObjectName):
                            {
                                IBJsonConverterInternal c = Provider.GetConverter(memData.Type);

                                object newVal = c.Deserialize_Internal(ref parseState, context, settings);

                                data.GameObjectName = (string)newVal;
                                break;
                            }
                        case nameof(GameObjectSceneData.SceneGameObjectIndex):
                            {
                                IBJsonConverterInternal c = Provider.GetConverter(memData.Type);

                                object newVal = c.Deserialize_Internal(ref parseState, context, settings);

                                data.SceneGameObjectIndex = (List<int>)newVal;
                                break;
                            }
                        case nameof(GameObjectSceneData.ComponentIndex):
                            {
                                IBJsonConverterInternal c = Provider.GetConverter(memData.Type);

                                object newVal = c.Deserialize_Internal(ref parseState, context, settings);

                                data.ComponentIndex = (int)newVal;
                                break;
                            }
                        case nameof(GameObjectSceneData.ComponentType):
                            {
                                IBJsonConverterInternal c = Provider.GetConverter(memData.Type);

                                object newVal = c.Deserialize_Internal(ref parseState, context, settings);

                                data.ComponentType = (Type)newVal;
                                break;
                            }
                        case nameof(GameObjectSceneData.ComponentValue):
                            {
                                componentStartTokenIndex = parseState.CurrentTokenIndex;
                                break;
                            }
                        default:
                            {
                                throw new Exception($"Field {key}doesn't exist in the struct");
                            }
                    }
                }

                // skip until the first comma or object end
                parseState.SkipUntil(ParserUtils.IsPropertyEndObject);

                if (parseState.CurrentToken.TokenType == JSONTokenType.OBJECT_END)
                {
                    parseState.CurrentTokenIndex++;
                    break;
                }
            }

            // load the component
            {
                Scene scene = SceneManager.GetSceneByPath(data.ScenePath);
                bool isValid = scene.IsValid();

                if (!isValid)
                {
                    Debug.LogError($"Scene is not valid when deserializing {data.GameObjectName} in scene {data.ScenePath}");
                }


                UnityEngine.GameObject[] gos = scene.GetRootGameObjects();

                GameObject curr = gos[data.SceneGameObjectIndex[0]];

                for (int i = 1; i < data.SceneGameObjectIndex.Count; i++)
                {
                    curr = curr.transform.GetChild(data.SceneGameObjectIndex[i]).gameObject;
                }

                Component[] allComp = curr.GetComponents<Component>();

                Component currComp = allComp[data.ComponentIndex];

                Type compType = currComp.GetType();
                IBJsonConverterInternal c = null;
                settings.HasCustomConverter(compType, out c);

                ParserState<JSONTokenType> parserStateCopy = parseState;
                parserStateCopy.CurrentTokenIndex = componentStartTokenIndex;
                object value = c.Deserialize_Internal(ref parserStateCopy, context, settings);
                data.ComponentValue = value;
                parseState.CurrentTokenIndex = parserStateCopy.CurrentTokenIndex;

                // skip until the first comma or object end
                parseState.SkipUntil(ParserUtils.IsPropertyEndObject);
                parseState.CurrentTokenIndex++;
            }

            return data;
        }
        public override object Populate_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            if (BJsonUtils.IsCachedOrNull(ref parseState, context, settings, out object cached))
            {
                return cached;
            }

            if (instance == null)
            {
                instance = CreateInstance_Internal();
            }

            context.Register(instance);

            int startTokenIndex = parseState.CurrentTokenIndex;

            // skip the first object start
            parseState.CurrentTokenIndex++;

            // prepare
            GameObjectSceneData data = new GameObjectSceneData();
            int componentStartTokenIndex = -1;


            while (parseState.CurrentTokenIndex < parseState.Tokens.Count || componentStartTokenIndex != -1)
            {
                Token<JSONTokenType> currentToken = parseState.Tokens[parseState.CurrentTokenIndex];

                // skip spaces
                if (currentToken.TokenType == JSONTokenType.SPACE)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // exit if object ended
                if (currentToken.TokenType == JSONTokenType.OBJECT_END)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // stp while no identitifer
                if (currentToken.TokenType != JSONTokenType.IDENTIFIER)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // key found
                string key = currentToken.ToString();

                // skip until the first colon
                parseState.SkipUntil(ParserUtils.IsColon);

                parseState.CurrentTokenIndex++;
                currentToken = parseState.Tokens[parseState.CurrentTokenIndex];

                // skip space
                parseState.SkipWhile(ParserUtils.IsSkippableSpace);

                if (MemberToDataDictionary.TryGetValue(key, out BMemberData memData))
                {
                    switch (key)
                    {
                        case nameof(GameObjectSceneData.ScenePath):
                            {
                                IBJsonConverterInternal c = Provider.GetConverter(memData.Type);

                                object oldVal = memData.MemberGetter(instance);

                                object newVal = c.Populate_Internal(oldVal, ref parseState, context, settings);

                                data.ScenePath = (string)newVal;
                                break;
                            }
                        case nameof(GameObjectSceneData.GameObjectName):
                            {
                                IBJsonConverterInternal c = Provider.GetConverter(memData.Type);

                                object oldVal = memData.MemberGetter(instance);

                                object newVal = c.Populate_Internal(oldVal, ref parseState, context, settings);

                                data.GameObjectName = (string)newVal;
                                break;
                            }
                        case nameof(GameObjectSceneData.SceneGameObjectIndex):
                            {
                                IBJsonConverterInternal c = Provider.GetConverter(memData.Type);

                                object oldVal = memData.MemberGetter(instance);

                                object newVal = c.Populate_Internal(oldVal, ref parseState, context, settings);

                                data.SceneGameObjectIndex = (List<int>)newVal;
                                break;
                            }
                        case nameof(GameObjectSceneData.ComponentType):
                            {
                                IBJsonConverterInternal c = Provider.GetConverter(memData.Type);

                                object oldVal = memData.MemberGetter(instance);

                                object newVal = c.Populate_Internal(oldVal, ref parseState, context, settings);

                                data.ComponentType = (Type)newVal;
                                break;
                            }
                        case nameof(GameObjectSceneData.ComponentIndex):
                            {
                                IBJsonConverterInternal c = Provider.GetConverter(memData.Type);

                                object oldVal = memData.MemberGetter(instance);

                                object newVal = c.Populate_Internal(oldVal, ref parseState, context, settings);

                                data.ComponentIndex = (int)newVal;
                                break;
                            }
                        case nameof(GameObjectSceneData.ComponentValue):
                            {
                                object newValue = PopulateComponent(ref parseState, context, settings, data);

                                data.ComponentValue = newValue;
                                break;
                            }
                        default:
                            {
                                throw new Exception($"Field {key}doesn't exist in the struct");
                            }
                    }
                }

                // the proble is here
                // to serializer is stopping at the propertyEnd of the RefMonobeaviour and not the properyEnd of the ComponenntValue
                // todo : populate in a correct way
                // skip until the first comma or object end
                parseState.SkipUntil(ParserUtils.IsPropertyEndObject);

                if (parseState.CurrentToken.TokenType == JSONTokenType.OBJECT_END)
                {
                    parseState.CurrentTokenIndex++;
                    break;
                }

            }

            return data;
        }

        private object PopulateComponent(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings, GameObjectSceneData data)
        {
            UnityObjectContext ctx = (UnityObjectContext)settings.CustomContext;

            GameObject[] gos = ctx.allSceneObjects[data.ScenePath];

            GameObject curr = gos[data.SceneGameObjectIndex[0]];

            for (int i = 1; i < data.SceneGameObjectIndex.Count; i++)
            {
                curr = curr.transform.GetChild(data.SceneGameObjectIndex[i]).gameObject;
            }

            Component[] allComp = curr.GetComponents<Component>();

            Component currComp = allComp[data.ComponentIndex];

            Type compType = currComp.GetType();

            settings.HasCustomConverter(compType, out IBJsonConverterInternal c);

            object oldVal = currComp;

            object newValue = c.Populate_Internal(oldVal, ref parseState, context, settings);

            return newValue;
        }
    }
}
