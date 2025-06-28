using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.System.CommandSystem;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.BISD.Editor.Commands
{
    public class CreateBehaviourFileCommand : CommandInstant<CreateBehaviourFileCommand>
    {
        private const string BEHAVIOUR_TEMPALTE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD/BISD Generator/Template.Behaviour.cs.txt";

        private const string REPLACE_MODEL_KEYWORD = "[MODELNAME]";
        private const string REPLACE_NAMESPACE_KEYWORD = "[NAMESPACE]";

        private readonly string modelName;
        private readonly string namespaceValue;
        private readonly string relativeFolderPath;

        public CreateBehaviourFileCommand(string modelName, string namespaceValue, string relativePath)
        {
            this.modelName = modelName;
            this.namespaceValue = namespaceValue;
            this.relativeFolderPath = relativePath;
        }

        public override void Execute()
        {
            string finalPath = EditorUtils.PathToProject + "/" + relativeFolderPath;
            TextAsset templateScriptAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(BEHAVIOUR_TEMPALTE);
            string scriptText = templateScriptAsset.text
                .Replace(REPLACE_MODEL_KEYWORD, modelName)
                .Replace(REPLACE_NAMESPACE_KEYWORD, namespaceValue);

            File.WriteAllText($"{finalPath}/{modelName}Behaviour.cs", scriptText);

            AssetDatabase.ImportAsset($"{relativeFolderPath}/{modelName}Behaviour.cs");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }
    }
}
