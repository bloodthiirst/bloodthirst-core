using Bloodthirst.Core.BISDSystem;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor.CodeGenerator;
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
    public class GameSaveCodeGenerator : ICodeGenerator
    {
        private const string START_TERM_CONST = "//GAMEDATA_START";
        private const string END_TERM_CONST = "//GAMEDATA_END";

        /// <summary>
        /// The start of the part of the script to inject the auto-generated observables
        /// </summary>
        private const string NAMESPACE_START_CONST = @"//NAMESPACE_START";

        /// <summary>
        /// The end of the part of the script to inject the auto-generated observables
        /// </summary>
        private const string NAMESPACE_END_CONST = @"//NAMESPACE_END";

        private readonly string[] filterForNamespaces = new string[]
        {
            "System",
            "System.Collections.Generic"
        };

        public bool ShouldInject(BISDInfoContainer container)
        {
            // if file doenst exist then we can't generate code
            if (container.GameSave?.ModelName == null)
            {
                Debug.LogWarning($"The gamesave file doesn't exist for the {container.ModelName} , consider regenerating it");
                return false;
            }

            // make sure all the fields have their corresponding save-state fields generated with correct names and types
            using (ListPool<MemberInfo>.Get(out List<MemberInfo> savable))
            using (ListPool<MemberInfo>.Get(out List<MemberInfo> generated))
            {
                savable.AddRange(GetFieldsInState(container));
                generated.AddRange(GetFieldsInGameData(container));

                for (int i = savable.Count - 1; i >= 0; i--)
                {
                    bool found = false;
                    MemberInfo s = savable[i];

                    for (int j = generated.Count - 1; j >= 0; j--)
                    {
                        MemberInfo g = generated[j];

                        CodeGenerationUtils.GenerateGameDataInfoFromStateField(s, out string n, out Type t);

                        // if we found matching fields , then we delete them since they are validated and go next
                        if (g.Name == n && ReflectionUtils.GetMemberType(g) == t)
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

        private IEnumerable<FieldInfo> GetFieldsInGameData(BISDInfoContainer typeInfo)
        {
            BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (FieldInfo f in typeInfo.GameSave.TypeRef.GetFields(flags))
            {
                yield return f;
            }
        }

        private IEnumerable<MemberInfo> GetFieldsInState(BISDInfoContainer typeInfo)
        {
            BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (FieldInfo f in typeInfo.State.TypeRef.GetFields(flags))
            {
                yield return f;
            }

            yield return typeInfo.State.TypeRef.BaseType.GetProperty("Data", BindingFlags.Public | BindingFlags.Instance);
            yield return typeInfo.State.TypeRef.BaseType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
        }

        public void InjectGeneratedCode(BISDInfoContainer typeInfo)
        {
            string oldScript = typeInfo.GameSave.TextAsset.text;
            TemplateCodeBuilder scriptBuilder = new TemplateCodeBuilder(oldScript);

            using (ListPool<MemberInfo>.Get(out List<MemberInfo> stateMembers))
            {
                stateMembers.AddRange(GetFieldsInState(typeInfo));

                // populate save from state
                {
                    ITextSection populateSaveFromStateSection = scriptBuilder.CreateSection(new StartEndTextSection(START_TERM_CONST, END_TERM_CONST));

                    populateSaveFromStateSection.AddWriter(new ReplaceAllTextWriter(string.Empty));
                    populateSaveFromStateSection.AddWriter(new AppendTextWriter(Environment.NewLine));
                    
                    foreach (MemberInfo member in stateMembers)
                    {
                        CodeGenerationUtils.GenerateGameDataInfoFromStateField(member, out string name, out Type type);

                        string typeNiceName = TypeUtils.GetNiceName(type);
                        string templateText = $"\t\tpublic {typeNiceName} {name};";

                        populateSaveFromStateSection.AddWriter(new AppendTextWriter($"{templateText}\n"));

                    }
                }

                // write namespaces
                {
                    ITextSection namespacesScriptSection = scriptBuilder
                        .CreateSection(new StartEndTextSection(NAMESPACE_START_CONST, NAMESPACE_END_CONST));

                    namespacesScriptSection
                        .AddWriter(new ReplaceAllTextWriter(string.Empty))
                        .AddWriter(new AppendTextWriter(Environment.NewLine));

                    using (HashSetPool<string>.Get(out HashSet<string> namespaceAdded))
                    {
                        for (int i = 0; i < stateMembers.Count; i++)
                        {
                            MemberInfo member = stateMembers[i];

                            Type memberType = TypeUtils.GetReturnType(member);

                            string namespaceAsString = memberType.Namespace;

                            if (string.IsNullOrEmpty(namespaceAsString))
                                continue;

                            if (filterForNamespaces.Contains(namespaceAsString))
                                continue;

                            if (!namespaceAdded.Add(memberType.Namespace))
                                continue;

                            namespacesScriptSection.AddWriter(new AppendTextWriter($"using {memberType.Namespace};\n"));
                        }
                    }
                }
            }

            string newScriptText = scriptBuilder.Build();

            // save
            File.WriteAllText(AssetDatabase.GetAssetPath(typeInfo.GameSave.TextAsset), newScriptText);

            // set dirty
            EditorUtility.SetDirty(typeInfo.GameSave.TextAsset);

        }

    }
}
