using Bloodthirst.Core.BISD.Editor.Commands;
using Bloodthirst.Editor.Commands;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public class BISDInfo
    {
        public BISDInfoContainer Container { get; set; }
        public string ModelName { get; set; }

        public Type TypeRef { get; set; }
        public TextAsset TextAsset { get; set; }
    }

    public class BISDGeneratorOnReloadScripts
    {
        /// <summary>
        /// Ending for state class files
        /// </summary>
        private const string STATE_FILE_ENDING = "State";

        /// <summary>
        /// Ending for instance class files
        /// </summary>
        private const string INSTANCE_FILE_ENDING = "Instance";

        /// <summary>
        /// Ending for behaviour class files
        /// </summary>
        private const string BEHAVIOUR_FILE_ENDING = "Behaviour";

        /// <summary>
        /// Ending for data class files
        /// </summary>
        private const string DATA_FILE_ENDING = "Data";

        /// <summary>
        /// Files that shouldn't be scanned for the code generation
        /// </summary>
        private static readonly string[] filterFiles =
        {
            nameof(BISDTag),
            nameof(BISDGeneratorOnReloadScripts)
        };

        [MenuItem("Bloodthirst Tools/BISD Pattern/Code Generation Refresh (COMPLETE REFRESH - EXPECT A FREEZE")]
        public static void MenuOption()
        {
            ExecuteCodeGeneration(false);
        }

        [MenuItem("Bloodthirst Tools/BISD Pattern/Code Generation Refresh (LAZY MODE - LESS EXPENSIVE)")]
        public static void MenuOptionLazy()
        {
            ExecuteCodeGeneration(true);
        }

        private static void ExecuteCodeGeneration(bool lazyGeneration = true)
        {
            // code generators
            List<ICodeGenerator> codeGenerators = new List<ICodeGenerator>()
            {
                new ObservableFieldsCodeGenerator(),
                new GameSaveCodeGenerator(),
                new GameSaveHandlerCodeGenerator()
            };

            ExtractBISDInfoCommand cmd = new ExtractBISDInfoCommand();

            CommandManagerEditor.RunInstant(cmd);

            // get models info
            Dictionary<string, BISDInfoContainer> typeList = cmd.Result;

            bool dirty = false;

            int affctedModels = 0;

            // run thorugh the models to apply the changes
            foreach (BISDInfoContainer t in typeList.Values)
            {
                bool modeldirty = false;

                foreach (ICodeGenerator generator in codeGenerators)
                {
                    modeldirty = modeldirty || CommandManagerEditor.RunInstant(new CommandExecuteCodeGenerator(t, generator, lazyGeneration));

                    dirty = dirty || modeldirty;
                }

                if (modeldirty)
                {
                    affctedModels++;
                }
            }

            Debug.Log($"BISD affected models : {affctedModels}");

            if (!dirty)
                return;


            // save the changes
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }




    }
}
