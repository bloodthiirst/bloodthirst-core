using Bloodthirst.Core.BISDSystem;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Bloodthirst.Core.Utils.StringExtensions;

namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public class GameSaveHandlerCodeGenerator : ICodeGenerator
    {
        private const string GET_STATE_START = "//GET_STATE_START";
        private const string GET_STATE_END = "//GET_STATE_END";

        private const string GET_SAVE_START = "//GET_SAVE_START";
        private const string GET_SAVE_END = "//GET_SAVE_END";
        
        private const string LINK_REFS_START = "//LINK_REFS_START";
        private const string LINK_REFS_END = "//LINK_REFS_END";


        /// <summary>
        /// path to templete to for GetSave method
        /// </summary>
        private const string GET_SAVE_FIELD_MANY_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD Generator/CodeGenerator/GameSaveHandler/Template.GetSave.GameSaveProperty.Many.cs.txt";
        private const string GET_SAVE_FIELD_SINGLE_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD Generator/CodeGenerator/GameSaveHandler/Template.GetSave.GameSaveProperty.Single.cs.txt";
        private const string GET_SAVE_FIELD_VALUE_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD Generator/CodeGenerator/GameSaveHandler/Template.GetSave.GameSaveProperty.Value.cs.txt";
        
        /// <summary>
        /// path to templete to for GetSave method
        /// </summary>
        private const string GET_STATE_FIELD_MANY_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD Generator/CodeGenerator/GameSaveHandler/Template.GetState.GameSaveProperty.Many.cs.txt";
        private const string GET_STATE_FIELD_SINGLE_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD Generator/CodeGenerator/GameSaveHandler/Template.GetState.GameSaveProperty.Single.cs.txt";
        private const string GET_STATE_FIELD_VALUE_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD Generator/CodeGenerator/GameSaveHandler/Template.GetState.GameSaveProperty.Value.cs.txt";

        private const string STATE_FIELD_NAME = "[STATE_FIELD_NAME]";
        private const string SAVE_FIELD_NAME = "[SAVE_FIELD_NAME]";
        private const string INSTANCE_TYPE = "[INSTANCE_TYPE]";

        public bool ShouldInject(BISDInfoContainer container)
        {
            if (container.GameSaveHandler?.ModelName == null)
                return false;

            // make sure all the fields have their corresponding save-state fields generated with correct names and types
            List<MemberInfo> savable = GetMembersInState(container);
            List<MemberInfo> generated = GetMembersInGameData(container);

            for (int i = savable.Count - 1; i >= 0; i--)
            {
                bool found = false;
                MemberInfo s = savable[i];

                for (int j = generated.Count - 1; j >= 0; j--)
                {
                    MemberInfo g = generated[j];

                    CodeGenerationUtils.GenerateGameDataInfoFromStateField(s, out string n, out Type t);

                    // if we found matching fields , then we delete them since they are validated and go next
                    if (g.Name.Equals(n) && ReflectionUtils.GetMemberType(g) == t)
                    {
                        found = true;
                        savable.RemoveAt(i);
                        generated.RemoveAt(j);
                        break;
                    }

                }

                if (!found)
                    return true;
            }

            return false;
        }

        private List<MemberInfo> GetMembersInGameData(BISDInfoContainer typeInfo)
        {
            List<MemberInfo> f = typeInfo.GameSave
                .TypeRef
                .GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Cast<MemberInfo>()
                .ToList();

            return f;
        }

        private List<MemberInfo> GetMembersInState(BISDInfoContainer typeInfo)
        {
            List<MemberInfo> f = typeInfo.State
                .TypeRef
                .GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Cast<MemberInfo>()
                .ToList();

            f.Add(typeInfo.State.TypeRef.BaseType.GetProperty("Data", BindingFlags.Public | BindingFlags.Instance));
            f.Add(typeInfo.State.TypeRef.BaseType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance));

            return f;
        }

        public void InjectGeneratedCode(BISDInfoContainer typeInfo)
        {
            List<MemberInfo> fields = GetMembersInState(typeInfo);

            string oldScript = typeInfo.GameSaveHandler.TextAsset.text;

            oldScript = GenerateGameDataFromStateCode(typeInfo, fields, oldScript);
            oldScript = GenerateGetStateFromGameDataCode(typeInfo, fields, oldScript);
            oldScript = GenerateLinkReferencesCode(typeInfo, fields, oldScript);

            // save
            File.WriteAllText(AssetDatabase.GetAssetPath(typeInfo.GameSaveHandler.TextAsset), oldScript);

            // set dirty
            EditorUtility.SetDirty(typeInfo.GameSaveHandler.TextAsset);

        }

        private string GenerateGetStateFromGameDataCode(BISDInfoContainer typeInfo, List<MemberInfo> members, string oldScript)
        {
            #region write the properties for the get state
            List<Tuple<SECTION_EDGE, int, int>> propsSections = oldScript.StringReplaceSection(GET_STATE_START, GET_STATE_END);

            int padding = 0;

            for (int i = 0; i < propsSections.Count - 1; i++)
            {
                Tuple<SECTION_EDGE, int, int> start = propsSections[i];
                Tuple<SECTION_EDGE, int, int> end = propsSections[i + 1];

                // if we have correct start and end
                // then do the replacing
                if (start.Item1 == SECTION_EDGE.START && end.Item1 == SECTION_EDGE.END)
                {
                    StringBuilder replacementText = new StringBuilder();

                    replacementText.Append(Environment.NewLine);
                    replacementText.Append(Environment.NewLine);

                    replacementText.Append("\t").Append("\t").Append("\t")
                        .Append("#region auto-generated code to get the State instance from the GameState")
                        .Append(Environment.NewLine);

                    // new instance
                    replacementText.Append("\t").Append("\t").Append("\t")
                        .Append($"{typeInfo.State.TypeRef.Name} state = new {typeInfo.State.TypeRef.Name}();")
                        .Append(Environment.NewLine);

                    // assign fields
                    foreach (MemberInfo mem in members)
                    {
                        string templateText = string.Empty;
                        templateText = CopyFieldGetState(mem, typeInfo);

                        replacementText.Append(templateText)
                        .Append(Environment.NewLine);
                    }

                    // return instance
                    replacementText.Append("\t").Append("\t").Append("\t")
                        .Append($"return state;")
                        .Append(Environment.NewLine);

                    replacementText.Append("\t").Append("\t").Append("\t").Append("#endregion")
                        .Append(Environment.NewLine);

                    replacementText
                    .Append(Environment.NewLine)
                    .Append("\t")
                    .Append("\t")
                    .Append("\t");

                    oldScript = oldScript.ReplaceBetween(start.Item3, end.Item2, replacementText.ToString());

                    int oldTextLength = end.Item2 - start.Item3;

                    padding += replacementText.Length - oldTextLength;
                }
            }
            #endregion

            return oldScript;
        }
        private string GenerateLinkReferencesCode(BISDInfoContainer typeInfo, List<MemberInfo> members, string oldScript)
        {
            #region write the properties for the get state referenced instances
            List<Tuple<SECTION_EDGE, int, int>> refsSections = oldScript.StringReplaceSection(LINK_REFS_START, LINK_REFS_END);

            int padding = 0;

            for (int i = 0; i < refsSections.Count - 1; i++)
            {
                Tuple<SECTION_EDGE, int, int> start = refsSections[i];
                Tuple<SECTION_EDGE, int, int> end = refsSections[i + 1];

                // if we have correct start and end
                // then do the replacing
                if (start.Item1 == SECTION_EDGE.START && end.Item1 == SECTION_EDGE.END)
                {
                    StringBuilder replacementText = new StringBuilder();

                    replacementText.Append(Environment.NewLine);
                    replacementText.Append(Environment.NewLine);

                    replacementText.Append("\t").Append("\t").Append("\t")
                        .Append("#region auto-generated code to link the instances back together in the linking pass of the loading")
                        .Append(Environment.NewLine);

                    // assign fields
                    foreach (MemberInfo mem in members)
                    {
                        string templateText = string.Empty;
                        templateText = CopyFieldRefGetSave(mem, typeInfo);

                        replacementText.Append(templateText)
                        .Append(Environment.NewLine);
                    }

                    replacementText.Append("\t").Append("\t").Append("\t").Append("#endregion")
                        .Append(Environment.NewLine);

                    replacementText
                    .Append(Environment.NewLine)
                    .Append("\t")
                    .Append("\t")
                    .Append("\t");

                    oldScript = oldScript.ReplaceBetween(start.Item3, end.Item2, replacementText.ToString());

                    int oldTextLength = end.Item2 - start.Item3;

                    padding += replacementText.Length - oldTextLength;
                }
            }
            #endregion

            return oldScript;
        }

        private static string GenerateGameDataFromStateCode(BISDInfoContainer typeInfo, List<MemberInfo> members, string oldScript)
        {

            #region write the properties for the get state for pure values
            List<Tuple<SECTION_EDGE, int, int>> propsSections = oldScript.StringReplaceSection(GET_SAVE_START, GET_SAVE_END);

            int padding = 0;

            for (int i = 0; i < propsSections.Count - 1; i++)
            {
                Tuple<SECTION_EDGE, int, int> start = propsSections[i];
                Tuple<SECTION_EDGE, int, int> end = propsSections[i + 1];

                // if we have correct start and end
                // then do the replacing
                if (start.Item1 == SECTION_EDGE.START && end.Item1 == SECTION_EDGE.END)
                {
                    StringBuilder replacementText = new StringBuilder();

                    replacementText.Append(Environment.NewLine);
                    replacementText.Append(Environment.NewLine);

                    replacementText.Append("\t").Append("\t").Append("\t")
                        .Append("#region auto-generated code to get the GameData instance from the State")
                        .Append(Environment.NewLine);

                    // new instance
                    replacementText.Append("\t").Append("\t").Append("\t")
                        .Append($"{typeInfo.GameSave.TypeRef.Name} save = new {typeInfo.GameSave.TypeRef.Name}();")
                        .Append(Environment.NewLine);

                    // assign fields
                    foreach (MemberInfo mem in members)
                    {
                        string templateText = string.Empty;
                        templateText = CopyFieldValueGetSave(mem , typeInfo);

                        replacementText.Append(templateText)
                        .Append(Environment.NewLine);
                    }

                    // return instance
                    replacementText.Append("\t").Append("\t").Append("\t")
                        .Append($"return save;")
                        .Append(Environment.NewLine);

                    replacementText.Append("\t").Append("\t").Append("\t").Append("#endregion")
                        .Append(Environment.NewLine);

                    replacementText
                    .Append(Environment.NewLine)
                    .Append("\t")
                    .Append("\t")
                    .Append("\t");

                    oldScript = oldScript.ReplaceBetween(start.Item3, end.Item2, replacementText.ToString());

                    int oldTextLength = end.Item2 - start.Item3;

                    padding += replacementText.Length - oldTextLength;
                }
            }
            #endregion

            return oldScript;
        }
        private static string CopyFieldValueGetSave(MemberInfo mem, BISDInfoContainer typeInfo)
        {
            List<Type> interfaces = ReflectionUtils.GetMemberType(mem)
                .GetInterfaces()
                .Where(i => i.IsGenericType)
                .Where(i =>
                {
                    Type t = i.GetGenericTypeDefinition();
                    return
                    t == typeof(IList<>) ||
                    t == typeof(ISet<>);
                })
                .Select(i =>
                {
                    Type genArg = i.GetGenericArguments()[0];
                    Type[] types = genArg.GetInterfaces();
                    Type isInstance = types.Contains(typeof(IEntityInstance)) ? genArg : null;

                    return isInstance;
                })
                .Where(i => i != null)
                .ToList();

            // many entity ref
            if (interfaces.Count != 0)
            {
                string templateText = AssetDatabase.LoadAssetAtPath<TextAsset>(GET_SAVE_FIELD_MANY_TEMPLATE).text;
                templateText = templateText.Replace(STATE_FIELD_NAME, mem.Name);
                templateText = templateText.Replace(SAVE_FIELD_NAME, $"{mem.Name}_Ids");

                return templateText;
            }


            // single entity ref
            else if (TypeUtils.IsSubTypeOf(ReflectionUtils.GetMemberType(mem), typeof(IEntityInstance)))
            {
                string templateText = AssetDatabase.LoadAssetAtPath<TextAsset>(GET_SAVE_FIELD_SINGLE_TEMPLATE).text;
                templateText = templateText.Replace(STATE_FIELD_NAME, mem.Name);
                templateText = templateText.Replace(SAVE_FIELD_NAME, $"{mem.Name}_Id");

                return templateText;
            }

            // other data
            {
                string templateText = AssetDatabase.LoadAssetAtPath<TextAsset>(GET_SAVE_FIELD_VALUE_TEMPLATE).text;
                templateText = templateText.Replace(STATE_FIELD_NAME, mem.Name);
                templateText = templateText.Replace(SAVE_FIELD_NAME, $"{mem.Name}");

                return templateText;
            }
        }
        private static string CopyFieldRefGetSave(MemberInfo mem, BISDInfoContainer typeInfo)
        {
            List<Type> interfaces = ReflectionUtils.GetMemberType(mem)
                .GetInterfaces()
                .Where(i => i.IsGenericType)
                .Where(i =>
                {
                    Type t = i.GetGenericTypeDefinition();
                    return
                    t == typeof(IList<>) ||
                    t == typeof(ISet<>);
                })
                .Select(i =>
                {
                    Type genArg = i.GetGenericArguments()[0];
                    Type[] types = genArg.GetInterfaces();
                    Type isInstance = types.Contains(typeof(IEntityInstance)) ? genArg : null;

                    return isInstance;
                })
                .Where(i => i != null)
                .ToList();

            // many entity ref
            if (interfaces.Count != 0)
            {
                string templateText = AssetDatabase.LoadAssetAtPath<TextAsset>(GET_STATE_FIELD_MANY_TEMPLATE).text;
                templateText = templateText.Replace(STATE_FIELD_NAME, mem.Name);
                templateText = templateText.Replace(SAVE_FIELD_NAME, $"{mem.Name}_Ids");
                templateText = templateText.Replace(INSTANCE_TYPE, $"{interfaces[0].Name}");

                return templateText;
            }


            // single entity ref
            else if (TypeUtils.IsSubTypeOf( ReflectionUtils.GetMemberType(mem), typeof(IEntityInstance)))
            {
                string templateText = AssetDatabase.LoadAssetAtPath<TextAsset>(GET_STATE_FIELD_SINGLE_TEMPLATE).text;
                templateText = templateText.Replace(STATE_FIELD_NAME, mem.Name);
                templateText = templateText.Replace(SAVE_FIELD_NAME, $"{mem.Name}_Id");
                templateText = templateText.Replace(INSTANCE_TYPE, $"{interfaces[0].Name}");

                return templateText;
            }

            // other data
            {
                return string.Empty;
            }
        }
        private static string CopyFieldGetState(MemberInfo mem, BISDInfoContainer typeInfo)
        {
            List<Type> interfaces = ReflectionUtils.GetMemberType(mem)
                .GetInterfaces()
                .Where(i => i.IsGenericType)
                .Where(i =>
                {
                    Type t = i.GetGenericTypeDefinition();
                    return
                    t == typeof(IList<>) ||
                    t == typeof(ISet<>);
                })
                .Select(i =>
                {
                    Type genArg = i.GetGenericArguments()[0];
                    Type[] types = genArg.GetInterfaces();
                    Type isInstance = types.Contains(typeof(IEntityInstance)) ? genArg : null;

                    return isInstance;
                })
                .Where(i => i != null)
                .ToList();

            // many entity ref
            if (interfaces.Count != 0)
            {
                return string.Empty;
            }


            // single entity ref
            else if (TypeUtils.IsSubTypeOf(ReflectionUtils.GetMemberType(mem), typeof(IEntityInstance)))
            {
                return string.Empty;
            }

            // other data
            {
                string templateText = AssetDatabase.LoadAssetAtPath<TextAsset>(GET_STATE_FIELD_VALUE_TEMPLATE).text;
                templateText = templateText.Replace(STATE_FIELD_NAME, mem.Name);
                templateText = templateText.Replace(SAVE_FIELD_NAME, $"{mem.Name}");

                return templateText;
            }
        }
    }
}
