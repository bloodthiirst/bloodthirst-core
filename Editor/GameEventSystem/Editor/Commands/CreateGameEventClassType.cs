using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.System.CommandSystem;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.GameEventSystem
{
    public class CreateGameEventClassType : CommandInstant<CreateGameEventClassType>
    {
        private const string TYPE_TEMPLATE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "GameEventSystem/Editor/Commands/Template.GameEventSystem.Type.cs.txt";

        private const string GAME_EVENT_ENUM_TYPE = "[GAME_EVENT_ENUM_TYPE]";
        private const string GAME_EVENT_ENUM_VALUE = "[GAME_EVENT_ENUM_VALUE]";
        private const string GAME_EVENT_CLASS_TYPE = "[GAME_EVENT_CLASS_TYPE]";
        private const string GAME_EVENT_CLASS_NAMESPACE = "[GAME_EVENT_CLASS_NAMESPACE]";


        public CreateGameEventClassType( GameEventSystemAsset asset , string enumName , string className)
        {
            string relativePath = AssetDatabase.GetAssetPath(asset);
            string absPath = EditorUtils.RelativeToAbsolutePath(relativePath);
            string getFolder = EditorUtils.GetFolderFromPath(absPath);

            NamespaceValue = asset.namespaceValue;

            AbsolutePath = getFolder + "/" + className + ".cs";
            NamespaceValue = asset.namespaceValue ;
            EnumType = asset.enumName;
            EnumName = enumName;
            ClassType = className;

        }

        public string AbsolutePath { get; }
        public string EnumType { get; }
        public string EnumName { get; }
        public string ClassType { get; }
        public string NamespaceValue { get; }

        public override void Execute()
        {
            string templateText = File.ReadAllText(TYPE_TEMPLATE);

            string resultText = templateText
                .Replace(GAME_EVENT_CLASS_NAMESPACE, NamespaceValue)
                .Replace(GAME_EVENT_ENUM_VALUE, EnumName)
                .Replace(GAME_EVENT_ENUM_TYPE, EnumType)
                .Replace(GAME_EVENT_CLASS_TYPE, ClassType);

            File.WriteAllText(AbsolutePath, resultText);

            string relativePath = EditorUtils.AbsoluteToRelativePath(AbsolutePath);
            
            AssetDatabase.ImportAsset(relativePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }
    }
}
