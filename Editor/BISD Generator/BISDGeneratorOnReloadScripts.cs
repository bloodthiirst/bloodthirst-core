using Bloodthirst.Core.BISD.Editor.Commands;
using Bloodthirst.Core.Consts;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor.Commands;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CSharp;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        [UnityEditor.Callbacks.DidReloadScripts(BloodthirstCoreConsts.BISD_OBSERVABLE_GENERATOR)]
        public static void OnReloadScripts()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

           // ExecuteCodeGeneration();
        }

        private static void ExecuteCodeGeneration(bool lazyGeneration = true)
        {
            // code generators
            List<ICodeGenerator> codeGenerators = new List<ICodeGenerator>()
            {
                new ObservableFieldsCodeGenerator(),
                new GameStateCodeGenerator(),
                new LoadSaveHandlerCodeGenerator()
            };

            ExtractBISDInfoCommand cmd = new ExtractBISDInfoCommand();

            CommandManagerEditor.Run(cmd);

            // get models info
            Dictionary<string, BISDInfoContainer> typeList = cmd.Result; 

            string[] models = typeList.Keys.ToArray();

            bool dirty = false;

            int affctedModels = 0;

            // run thorugh the models to apply the changes
            foreach (string model in models)
            {
                BISDInfoContainer typeInfo = typeList[model];

                bool modeldirty = false;

                foreach (ICodeGenerator generator in codeGenerators)
                {
                    if (!lazyGeneration || generator.ShouldInject(typeInfo) )
                    {
                        dirty = true;
                        modeldirty = true;
                        generator.InjectGeneratedCode(typeInfo);
                    }
                }

                if(modeldirty)
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
