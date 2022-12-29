using Bloodthirst.Core.BISDSystem;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.CodeGenerator;
#if ODIN_INSPECTOR
using Sirenix.Utilities;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

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
        private const string GET_SAVE_FIELD_MANY_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD/BISD Generator/CodeGenerator/GameSaveHandler/Template.GetSave.GameSaveProperty.Many.cs.txt";
        private const string GET_SAVE_FIELD_SINGLE_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD/BISD Generator/CodeGenerator/GameSaveHandler/Template.GetSave.GameSaveProperty.Single.cs.txt";
        private const string GET_SAVE_FIELD_VALUE_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD/BISD Generator/CodeGenerator/GameSaveHandler/Template.GetSave.GameSaveProperty.Value.cs.txt";

        /// <summary>
        /// path to templete to for GetSave method
        /// </summary>
        private const string GET_STATE_FIELD_MANY_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD/BISD Generator/CodeGenerator/GameSaveHandler/Template.GetState.GameSaveProperty.Many.cs.txt";
        private const string GET_STATE_FIELD_SINGLE_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD/BISD Generator/CodeGenerator/GameSaveHandler/Template.GetState.GameSaveProperty.Single.cs.txt";
        private const string GET_STATE_FIELD_VALUE_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD/BISD Generator/CodeGenerator/GameSaveHandler/Template.GetState.GameSaveProperty.Value.cs.txt";

        private const string STATE_FIELD_NAME = "[STATE_FIELD_NAME]";
        private const string SAVE_FIELD_NAME = "[SAVE_FIELD_NAME]";
        private const string INSTANCE_TYPE = "[INSTANCE_TYPE]";

        public bool ShouldInject(BISDInfoContainer container)
        {
            if (container.GameSaveHandler?.ModelName == null)
                return false;

            // make sure all the fields have their corresponding save-state fields generated with correct names and types
            using (ListPool<MemberInfo>.Get(out List<MemberInfo> generated))
            using (ListPool<MemberInfo>.Get(out List<MemberInfo> savable))
            {
                savable.AddRange(GetMembersInState(container));
                generated.AddRange(GetMembersInGameData(container));

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
        }

        private IEnumerable<MemberInfo> GetMembersInGameData(BISDInfoContainer typeInfo)
        {
            BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            return typeInfo.GameSave
                .TypeRef
                .GetFields(flags)
                .Cast<MemberInfo>();

        }

        private IEnumerable<MemberInfo> GetMembersInState(BISDInfoContainer typeInfo)
        {
            BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            IEnumerable<MemberInfo> f = typeInfo.State
                .TypeRef
                .GetFields(flags)
                .Cast<MemberInfo>();

            foreach (MemberInfo m in f)
            {
                yield return m;
            }

            yield return typeInfo.State.TypeRef.BaseType.GetProperty("Data", BindingFlags.Public | BindingFlags.Instance);
            yield return typeInfo.State.TypeRef.BaseType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);

        }

        public void InjectGeneratedCode(BISDInfoContainer typeInfo)
        {
            string oldScript = typeInfo.GameSaveHandler.TextAsset.text;

            TemplateCodeBuilder scriptBuilder = new TemplateCodeBuilder(oldScript);

            using (ListPool<MemberInfo>.Get(out List<MemberInfo> stateMembers))
            {
                stateMembers.AddRange(GetMembersInState(typeInfo));

                // populate save from state
                {
                    ITextSection populateSaveFromStateSection = scriptBuilder.CreateSection(new StartEndTextSection(GET_SAVE_START, GET_SAVE_END));

                    populateSaveFromStateSection
                        .AddWriter(new ReplaceAllTextWriter(string.Empty))
                        .AddWriter(new AppendTextWriter($"\n\t\t\t{typeInfo.GameSave.TypeRef.Name} save = new {typeInfo.GameSave.TypeRef.Name}();\n"));

                    foreach (MemberInfo mem in stateMembers)
                    {
                        populateSaveFromStateSection.AddWriter(new AppendTextWriter($"{CopyFieldValueGetSave(mem, typeInfo)}\n"));

                    }

                    populateSaveFromStateSection.AddWriter(new AppendTextWriter("\t\t\treturn save;"));

                    populateSaveFromStateSection
                        .AddWriter(new AppendTextWriter(Environment.NewLine))
                        .AddWriter(new AppendTextWriter("\t\t\t"));
                }

                // populate state from save
                {
                    ITextSection populateStateFromSaveSection = scriptBuilder.CreateSection(new StartEndTextSection(GET_STATE_START, GET_STATE_END));

                    populateStateFromSaveSection
                        .AddWriter(new ReplaceAllTextWriter(string.Empty))
                        .AddWriter(new AppendTextWriter($"\n\t\t\t{typeInfo.State.TypeRef.Name} state = new {typeInfo.State.TypeRef.Name}();\n"));

                    foreach (MemberInfo mem in stateMembers)
                    {
                        populateStateFromSaveSection.AddWriter(new AppendTextWriter($"{CopyFieldGetState(mem, typeInfo)}\n"));

                    }

                    populateStateFromSaveSection.AddWriter(new AppendTextWriter("\t\t\treturn state;"));

                    populateStateFromSaveSection
                        .AddWriter(new AppendTextWriter(Environment.NewLine))
                        .AddWriter(new AppendTextWriter("\t\t\t"));
                }

                // populate link refs
                {
                    ITextSection linkRefsSection = scriptBuilder.CreateSection(new StartEndTextSection(LINK_REFS_START, LINK_REFS_END));

                    linkRefsSection
                        .AddWriter(new ReplaceAllTextWriter(string.Empty))
                        .AddWriter(new AppendTextWriter($"\n"));

                    foreach (MemberInfo mem in stateMembers)
                    {
                        if (TryGetLinkRefCode(mem, typeInfo, out string linkRefCode))
                        {
                            linkRefsSection.AddWriter(new AppendTextWriter($"{linkRefCode}"));
                        }
                    }

                    linkRefsSection
                        .AddWriter(new AppendTextWriter(Environment.NewLine))
                        .AddWriter(new AppendTextWriter("\t\t\t"));
                }
            }

            string newScriptText = scriptBuilder.Build();

            // save
            File.WriteAllText(AssetDatabase.GetAssetPath(typeInfo.GameSaveHandler.TextAsset), newScriptText);

            // set dirty
            EditorUtility.SetDirty(typeInfo.GameSaveHandler.TextAsset);
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
        private static bool TryGetLinkRefCode(MemberInfo mem, BISDInfoContainer typeInfo , out string linkRefCode)
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

                linkRefCode = templateText;
                return true;
            }


            // single entity ref
            else if (TypeUtils.IsSubTypeOf(ReflectionUtils.GetMemberType(mem), typeof(IEntityInstance)))
            {
                string templateText = AssetDatabase.LoadAssetAtPath<TextAsset>(GET_STATE_FIELD_SINGLE_TEMPLATE).text;
                templateText = templateText.Replace(STATE_FIELD_NAME, mem.Name);
                templateText = templateText.Replace(SAVE_FIELD_NAME, $"{mem.Name}_Id");
                templateText = templateText.Replace(INSTANCE_TYPE, $"{interfaces[0].Name}");

                linkRefCode = templateText;
                return true;
            }

            // other data
            {
                linkRefCode = string.Empty;
                return false;
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
