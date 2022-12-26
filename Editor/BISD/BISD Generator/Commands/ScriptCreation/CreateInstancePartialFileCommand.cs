using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.System.CommandSystem;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.BISD.Editor.Commands
{
    public class CreateInstancePartialFileCommand : CommandInstant<CreateInstancePartialFileCommand>
    {
        private const string INSTANCE_PARTIAL_TEMPALTE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD/BISD Generator/Template.Instance.Partial.cs.txt";
        private const string REPLACE_KEYWORD = "[MODELNAME]";

        private readonly string modelName;
        private readonly string relativeFolderPath;

        public CreateInstancePartialFileCommand(string modelName , string relativeFolderPath)
        {
            this.modelName = modelName;
            this.relativeFolderPath = relativeFolderPath;
        }

        protected override void Execute()
        {
            string finalPath = EditorUtils.PathToProject + "/" + relativeFolderPath;
            string scriptText = AssetDatabase.LoadAssetAtPath<TextAsset>(INSTANCE_PARTIAL_TEMPALTE).text.Replace(REPLACE_KEYWORD, modelName);
            
            File.WriteAllText($"{finalPath}/{modelName}Instance.Partial.cs", scriptText);
            
            AssetDatabase.ImportAsset($"{relativeFolderPath}/{modelName}Instance.Partial.cs");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }
    }
}
