using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.System.CommandSystem;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.BISD.Editor.Commands
{
    public class CreateStateFileCommand : CommandInstant<CreateStateFileCommand>
    {
        private const string STATE_TEMPALTE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD Generator/Template.State.cs.txt";

        private const string REPLACE_KEYWORD = "[MODELNAME]";

        private readonly string modelName;
        private readonly string relativeFolderPath;

        public CreateStateFileCommand(string modelName , string relativeFolderPath)
        {
            this.modelName = modelName;
            this.relativeFolderPath = relativeFolderPath;
        }

        protected override void Execute()
        {
            string finalPath = EditorUtils.PathToProject + relativeFolderPath;
            string scriptText = AssetDatabase.LoadAssetAtPath<TextAsset>(STATE_TEMPALTE).text.Replace(REPLACE_KEYWORD, modelName);
            
            File.WriteAllText($"{finalPath}/{modelName}State.cs", scriptText);
            
            AssetDatabase.ImportAsset($"{relativeFolderPath}/{modelName}State.cs");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }
    }
}
