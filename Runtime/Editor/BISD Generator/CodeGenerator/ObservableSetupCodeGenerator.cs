using Assets.Scripts.Game;
using Packages.com.bloodthirst.bloodthirst_core.Runtime.Utils;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Packages.com.bloodthirst.bloodthirst_core.Runtime.Utils.StringExtensions;

namespace Packages.com.bloodthirst.bloodthirst_core.Runtime.Editor.BISD_Generator.CodeGenerator
{
    public class ObservableSetupCodeGenerator : ICodeGenerator
    {
        private const string START_CONST = @"//REGISTER_OBSERVABLES_START";

        private const string END_CONSt = @"//REGISTER_OBSERVABLES_END";

        public void InjectGeneratedCode(Container<Type> TypeList, Container<TextAsset> TextList)
        {
            // fields
            FieldInfo[] fields = TypeList.State
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => !f.Name.Equals("data"))
                .Where(f => !f.Name.Equals("id"))
                .ToArray();


            // do the generation

            // separate the parts

            string oldScript = TextList.Instance.text;

            List<Tuple<SECTION_EDGE, int, int>> sections = oldScript.StringReplaceSection(START_CONST, END_CONSt);

            int padding = 0;

            for (int i = 0; i < sections.Count - 1; i++)
            {
                Tuple<SECTION_EDGE, int, int> start = sections[i];
                Tuple<SECTION_EDGE, int, int> end = sections[i + 1];

                // if we have correct start and end
                // then do the replacing
                if (start.Item1 == SECTION_EDGE.START && end.Item1 == SECTION_EDGE.END)
                {
                    var replacementText = new StringBuilder();
                    replacementText.Append(Environment.NewLine);
                    replacementText.Append(Environment.NewLine);

                    replacementText.Append("\t").Append("\t").Append("#region state accessors");

                    foreach (FieldInfo field in fields)
                    {
                        replacementText
                        .Append("\t")
                        .Append("\t")
                        .Append("\t")
                        .Append(FieldToObserverName(field))
                        .Append(" = new Observable<")
                        .Append(field.FieldType.GetNiceName())
                        .Append(">(")
                        .Append("State.")
                        .Append(field.Name)
                        .Append(");")
                        .Append(Environment.NewLine)
                        .Append(Environment.NewLine);
                    }

                    replacementText.Append("\t").Append("\t").Append("#endregion state accessors");

                    replacementText
                    .Append(Environment.NewLine)
                    .Append(Environment.NewLine)
                    .Append("\t")
                    .Append("\t")
                    .Append("\t");

                    oldScript = oldScript.ReplaceBetween(start.Item3, end.Item2, replacementText.ToString());

                    int oldTextLength = end.Item2 - start.Item3;

                    padding += replacementText.Length - oldTextLength;
                }
            }

            // save
            File.WriteAllText(AssetDatabase.GetAssetPath(TextList.Instance), oldScript);

            // set dirty
            EditorUtility.SetDirty(TextList.Instance);
        }

        private static string FieldToObserverName(FieldInfo field)
        {
            StringBuilder sb = new StringBuilder(field.GetNiceName());
            sb.Append("Prop");
            sb[0] = Char.ToUpper(sb[0]);
            return sb.ToString();
        }

        public bool ShouldInject(Container<Type> TypeList, Container<TextAsset> TextList)
        {
            return true;
        }
    }
}
