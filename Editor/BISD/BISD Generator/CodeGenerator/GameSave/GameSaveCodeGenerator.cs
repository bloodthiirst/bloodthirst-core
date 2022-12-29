using Bloodthirst.Core.BISDSystem;
using Bloodthirst.Core.Utils;
#if ODIN_INSPECTOR
	using Sirenix.Utilities;
#endif
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
    public class GameSaveCodeGenerator : ICodeGenerator
    {
        private const string START_TERM_CONST = "//GAMEDATA_START";
        private const string END_TERM_CONST = "//GAMEDATA_END";

        public bool ShouldInject(BISDInfoContainer container)
        {
            // if file doenst exist then we can't generate code
            if (container.GameSave?.ModelName == null)
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
            if (!ShouldInject(typeInfo))
                return;

            List<MemberInfo> members = GetMembersInState(typeInfo);

            string oldScript = typeInfo.GameSave.TextAsset.text;

            #region write the properties for the observables in the state
            List<SectionInfo> propsSections = oldScript.StringReplaceSection(START_TERM_CONST, END_TERM_CONST);

            int padding = 0;

            for (int i = 0; i < propsSections.Count - 1; i++)
            {
                SectionInfo start = propsSections[i];
                SectionInfo end = propsSections[i + 1];

                // if we have correct start and end
                // then do the replacing
                if (start.sectionEdge == SECTION_EDGE.START && end.sectionEdge == SECTION_EDGE.END)
                {
                    StringBuilder replacementText = new StringBuilder();

                    replacementText.Append(Environment.NewLine);
                    replacementText.Append(Environment.NewLine);

                    replacementText.Append("\t").Append("\t")
                        .Append("#region gamesave accessors")
                        .Append(Environment.NewLine)
                        .Append(Environment.NewLine);


                    foreach (MemberInfo mem in members)
                    {
                        string templateText = string.Empty;
                        CodeGenerationUtils.GenerateGameDataInfoFromStateField(mem, out string name, out Type type);

                        templateText = $"public { TypeUtils.GetNiceName(type)} {name};";

                        replacementText.Append(templateText)
                        .Append(Environment.NewLine)
                        .Append(Environment.NewLine);
                    }


                    replacementText.Append("\t").Append("\t").Append("#endregion gamesave accessors");

                    replacementText
                    .Append(Environment.NewLine)
                    .Append(Environment.NewLine)
                    .Append("\t")
                    .Append("\t");

                    oldScript = oldScript.ReplaceBetween(start.endIndex, end.startIndex, replacementText.ToString());

                    int oldTextLength = end.startIndex - start.endIndex;

                    padding += replacementText.Length - oldTextLength;
                }
            }
            #endregion

            // save
            File.WriteAllText(AssetDatabase.GetAssetPath(typeInfo.GameSave.TextAsset), oldScript);

            // set dirty
            EditorUtility.SetDirty(typeInfo.GameSave.TextAsset);

        }

    }
}
