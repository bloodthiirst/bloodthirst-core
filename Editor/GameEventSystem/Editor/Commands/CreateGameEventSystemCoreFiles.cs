using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.System.CommandSystem;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.GameEventSystem
{
    public class CreateGameEventSystemCoreFiles : CommandInstant<CreateGameEventSystemCoreFiles>
    {
        private const string ENUM_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "GameEventSystem/Editor/Commands/Template.GameEventSystem.Enum.cs.txt";

        private const string REPLACE_ENUM_NAME = "[ENUM_NAME]";

        private const string REPLACE_NAMEPSACE = "[NAMESPACE]";


        public CreateGameEventSystemCoreFiles(GameEventSystemAsset asset  , string forlderToSaveIn)
        {
            Asset = asset;
            EnumName = asset.enumName;
            NamespaceValue = asset.namespaceValue;
            ScriptPath = forlderToSaveIn + "/" + EnumName + ".cs";
        }

        public GameEventSystemAsset Asset { get; }
        public string ScriptPath { get; }
        public string EnumName { get; }
        public string NamespaceValue { get; }

        public override void Execute()
        {
            string templateText = File.ReadAllText(ENUM_TEMPLATE);

            string resultText = templateText
                .Replace(REPLACE_NAMEPSACE, NamespaceValue)
                .Replace(REPLACE_ENUM_NAME, EnumName);

            File.WriteAllText(ScriptPath, resultText);

            string relativePath = EditorUtils.AbsoluteToRelativePath(ScriptPath);
            
            AssetDatabase.ImportAsset(relativePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }
    }
}
