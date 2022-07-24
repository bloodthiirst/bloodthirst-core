using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.GameEventSystem
{
    public class RegenerateEnumScriptCommand : CommandInstant<RegenerateEnumScriptCommand>
    {
        private const string ENUM_ID_START = "//GENERATED_START";
        private const string ENUM_ID_END = "//GENERATED_END";


        public RegenerateEnumScriptCommand(GameEventSystemAsset asset)
        {
            Asset = asset;
        }

        public GameEventSystemAsset Asset { get; }

        protected override void Execute()
        {
            List<MonoScript> allScripts = EditorUtils.FindScriptAssets();

            MonoScript enumScript = allScripts
                .Where(m => m != null)
                .FirstOrDefault(m => m.name == Asset.enumName);


            string reslativeToAbsolute = AssetDatabase.GetAssetPath(enumScript);
            string oldScript = File.ReadAllText(reslativeToAbsolute);

            List<Tuple<SECTION_EDGE, int, int>> propsSections = oldScript.StringReplaceSection(ENUM_ID_START, ENUM_ID_END);

            for (int i = 0; i < propsSections.Count - 1; i++)
            {
                Tuple<SECTION_EDGE, int, int> start = propsSections[i];
                Tuple<SECTION_EDGE, int, int> end = propsSections[i + 1];

                // if we have correct start and end
                // then do the replacing
                if (start.Item1 != SECTION_EDGE.START || end.Item1 != SECTION_EDGE.END)
                {
                    continue;
                }

                StringBuilder replacementText = new StringBuilder();

                replacementText.Append(Environment.NewLine);
                replacementText.Append(Environment.NewLine);

                replacementText.Append("\t").Append("\t")
                    .Append("#region generated gameevent IDs")
                    .Append(Environment.NewLine)
                    .Append(Environment.NewLine);


                for (int j = 0; j < Asset.Count; j++)
                {
                    string comment = string.Empty;
                    string templateText = string.Empty;


                    templateText = $"{Asset[j].enumValue},";

                    replacementText.Append("\t").Append("\t").Append(comment)
                        .Append(Environment.NewLine);

                    replacementText.Append("\t").Append("\t").Append(templateText)
                    .Append(Environment.NewLine)
                    .Append(Environment.NewLine);
                }


                replacementText.Append("\t").Append("\t").Append("#endregion generated gameevent IDs");

                replacementText
                .Append(Environment.NewLine)
                .Append(Environment.NewLine)
                .Append("\t")
                .Append("\t");

                oldScript = oldScript.ReplaceBetween(start.Item3, end.Item2, replacementText.ToString());

                int oldTextLength = end.Item2 - start.Item3;
            }

            File.WriteAllText(reslativeToAbsolute, oldScript);

            EditorUtility.SetDirty(enumScript);
            EditorUtility.SetDirty(Asset);

            AssetDatabase.Refresh();
        }
    }
}
