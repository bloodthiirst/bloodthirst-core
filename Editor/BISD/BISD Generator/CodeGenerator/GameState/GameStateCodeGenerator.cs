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
    public class GameStateCodeGenerator : ICodeGenerator
    {
        private const string START_TERM_CONST = "//GAMEDATA_START";
        private const string END_TERM_CONST = "//GAMEDATA_END";

        public bool ShouldInject(BISDInfoContainer container)
        {
            return container.GameSave.ModelName != null;
        }

        private List<MemberInfo> GetSaveableMembers(BISDInfoContainer typeInfo)
        {
            List<MemberInfo> f = new List<MemberInfo>();

            FieldInfo[] childStateFields = typeInfo.State.TypeRef.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            f.Add(typeInfo.State.TypeRef.BaseType.GetProperty("Data", BindingFlags.Public | BindingFlags.Instance));
            f.Add(typeInfo.State.TypeRef.BaseType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance));
            f.AddRange(childStateFields);

            return f;
        }

        public void InjectGeneratedCode(BISDInfoContainer typeInfo)
        {
            if (!ShouldInject(typeInfo))
                return;

            List<MemberInfo> members = GetSaveableMembers(typeInfo);

            string oldScript = typeInfo.GameSave.TextAsset.text;

            #region write the properties for the observables in the state
            List<Tuple<SECTION_EDGE, int, int>> propsSections = oldScript.StringReplaceSection(START_TERM_CONST, END_TERM_CONST);

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

                    replacementText.Append("\t").Append("\t")
                        .Append("#region gamestate accessors")
                        .Append(Environment.NewLine)
                        .Append(Environment.NewLine);


                    foreach (MemberInfo mem in members)
                    {
                        string templateText = string.Empty;
                        templateText = GenerateSaveFieldForStateField(mem);

                        replacementText.Append(templateText)
                        .Append(Environment.NewLine)
                        .Append(Environment.NewLine);
                    }


                    replacementText.Append("\t").Append("\t").Append("#endregion gamestate accessors");

                    replacementText
                    .Append(Environment.NewLine)
                    .Append(Environment.NewLine)
                    .Append("\t")
                    .Append("\t");

                    oldScript = oldScript.ReplaceBetween(start.Item3, end.Item2, replacementText.ToString());

                    int oldTextLength = end.Item2 - start.Item3;

                    padding += replacementText.Length - oldTextLength;
                }
            }
            #endregion

            // save
            File.WriteAllText(AssetDatabase.GetAssetPath(typeInfo.GameSave.TextAsset), oldScript);

            // set dirty
            EditorUtility.SetDirty(typeInfo.GameSave.TextAsset);

        }

        private static string GenerateSaveFieldForStateField(MemberInfo mem)
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
                return $"\t \t public List<int> { mem.Name }_Ids;";
            }

            // single entity ref
            else if (TypeUtils.IsSubTypeOf(ReflectionUtils.GetMemberType(mem), typeof(IEntityInstance)))
            {
                return $"\t \t public int { mem.Name }_Id;";
            }

            // other data
            return $"\t \t public { ReflectionUtils.GetMemberType(mem).Name } { mem.Name }_Value;";

        }

    }
}
