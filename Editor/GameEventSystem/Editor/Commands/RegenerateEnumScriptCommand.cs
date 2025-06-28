using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.CodeGenerator;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public override void Execute()
        {
            List<MonoScript> allScripts = EditorUtils.FindScriptAssets();

            MonoScript enumScript = allScripts
                .Where(m => m != null)
                .FirstOrDefault(m => m.name == Asset.enumName);


            string reslativeToAbsolute = AssetDatabase.GetAssetPath(enumScript);
            string oldScript = File.ReadAllText(reslativeToAbsolute);


            TemplateCodeBuilder scriptBuilder = new TemplateCodeBuilder(oldScript);

            // write enum values
            {
                ITextSection enumValuesWriter = scriptBuilder
                    .CreateSection(new StartEndTextSection(ENUM_ID_START, ENUM_ID_END));
                enumValuesWriter
                    .AddWriter(new ReplaceAllTextWriter(string.Empty))
                    .AddWriter(new AppendTextWriter(Environment.NewLine));

                for (int j = 0; j < Asset.Count; j++)
                {
                    enumValuesWriter.AddWriter(new AppendTextWriter($"\t\t{Asset[j].enumValue}{Environment.NewLine}"));
                }

                enumValuesWriter
                    .AddWriter(new AppendTextWriter(Environment.NewLine))
                    .AddWriter(new AppendTextWriter("\t\t"));
            }

            string newScriptText = scriptBuilder.Build();

            File.WriteAllText(reslativeToAbsolute, newScriptText);

            EditorUtility.SetDirty(enumScript);
            EditorUtility.SetDirty(Asset);

            AssetDatabase.Refresh();
        }
    }
}
