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
        private const string BEHAVIOUR_TEMPALTE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD Generator/Template.Behaviour.cs.txt";

        private const string REPLACE_KEYWORD = "[MODELNAME]";

        private readonly string modelName;
        private readonly string relativeFolderPath;

        public CreateBehaviourFileCommand(string modelName , string relativeFolderPath)
        {
            this.modelName = modelName;
            this.relativeFolderPath = relativeFolderPath;
        }

        protected override void Execute()
        {
            string finalPath = EditorUtils.PathToProject + "/" + relativeFolderPath;
            string scriptText = AssetDatabase.LoadAssetAtPath<TextAsset>(BEHAVIOUR_TEMPALTE).text.Replace(REPLACE_KEYWORD, modelName);
            
            File.WriteAllText($"{finalPath}/{modelName}Behaviour.cs", scriptText);
            
            AssetDatabase.ImportAsset($"{relativeFolderPath}/{modelName}Behaviour.cs");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }
    }
}
