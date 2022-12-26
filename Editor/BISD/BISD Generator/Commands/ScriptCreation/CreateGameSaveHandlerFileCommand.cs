using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.System.CommandSystem;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.BISD.Editor.Commands
{
    public class CreateGameSaveHandlerFileCommand : CommandInstant<CreateGameSaveHandlerFileCommand>
    {
        private const string LOAD_SAVE_TEMPALTE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD/BISD Generator/Template.GameSaveHandler.cs.txt";

        private const string REPLACE_KEYWORD = "[MODELNAME]";

        private readonly string modelName;
        private readonly string relativeFolderPath;

        public CreateGameSaveHandlerFileCommand(string modelName , string relativeFolderPath)
        {
            this.modelName = modelName;
            this.relativeFolderPath = relativeFolderPath;
        }

        protected override void Execute()
        {
            string finalPath = EditorUtils.PathToProject + "/" + relativeFolderPath;
            string scriptText = AssetDatabase.LoadAssetAtPath<TextAsset>(LOAD_SAVE_TEMPALTE).text.Replace(REPLACE_KEYWORD, modelName);
            
            File.WriteAllText($"{finalPath}/{modelName}GameSaveHandler.cs", scriptText);
            
            AssetDatabase.ImportAsset($"{relativeFolderPath}/{modelName}GameSaveHandler.cs");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }
    }
}
