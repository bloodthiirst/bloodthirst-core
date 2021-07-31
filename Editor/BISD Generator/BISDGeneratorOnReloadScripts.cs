using Bloodthirst.Core.Consts;
using Bloodthirst.Core.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CSharp;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public class BISDInfo
    {
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

            // get models info
            Dictionary<string, Container> TypeList = null;

            ExtractBISDInfo(ref TypeList);

            string[] models = TypeList.Keys.ToArray();

            bool dirty = false;

            int affctedModels = 0;

            // run thorugh the models to apply the changes
            foreach (string model in models)
            {
                Container typeInfo = TypeList[model];

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

        /// <summary>
        /// Extracts info about the classes that follow the BISD pattern
        /// </summary>
        /// <param name="TypeList"></param>
        /// <param name="TextList"></param>
        private static void ExtractBISDInfo(ref Dictionary<string, Container> TypeList)
        {
            IReadOnlyList<Type> allTypes = TypeUtils.AllTypes;

            TypeList = new Dictionary<string, Container>();

            CSharpCodeProvider provider = new CSharpCodeProvider();

            Dictionary<TextAsset, Type> fileToType = new Dictionary<TextAsset, Type>();

            foreach (TextAsset txt in EditorUtils.FindTextAssets())
            {
                string relativePath = AssetDatabase.GetAssetPath(txt);

                string systemPath = EditorUtils.PathToProject + relativePath;

                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(txt.text);

                CompilationUnitSyntax root = syntaxTree.GetRoot() as CompilationUnitSyntax;

                List<ClassDeclarationSyntax> classesList = new List<ClassDeclarationSyntax>();

                // get classes inside namespaces
                root.Members.OfType<NamespaceDeclarationSyntax>().ForEach(n => classesList.AddRange(n.Members.OfType<ClassDeclarationSyntax>()));

                // get classes in file directly
                root.Members.OfType<ClassDeclarationSyntax>().ForEach(c => classesList.Add(c));

                foreach (ClassDeclarationSyntax c in classesList)
                {
                    AttributeSyntax attrib = c.AttributeLists.SelectMany(att => att.Attributes).FirstOrDefault(a => a.Name.ToString() == nameof(BISDTag));

                    if (attrib == null)
                        continue;

                    string modelName = default;

                    string modelEnum = default;

                    // try get model name
                    {
                        AttributeArgumentSyntax attrArgName = attrib.ArgumentList.Arguments[0];

                        SyntaxKind syntaxKind = attrArgName.Expression?.Kind() ?? SyntaxKind.None;
                        if (syntaxKind == SyntaxKind.StringLiteralExpression)
                        {
                            LiteralExpressionSyntax modelNameSyntax = attrArgName.Expression as LiteralExpressionSyntax;
                            modelName = modelNameSyntax.Token.ValueText;
                        }
                    }

                    // try get model TYPE
                    {
                        AttributeArgumentSyntax attrArgType = attrib.ArgumentList.Arguments[1];

                        SyntaxKind syntaxKind = attrArgType.Expression?.Kind() ?? SyntaxKind.None;
                        if (syntaxKind == SyntaxKind.SimpleMemberAccessExpression)
                        {
                            MemberAccessExpressionSyntax modelNameSyntax = attrArgType.Expression as MemberAccessExpressionSyntax;
                            IdentifierNameSyntax enumSyntax = modelNameSyntax.Expression as IdentifierNameSyntax;

                            // enum type as string
                            string enumName = enumSyntax.Identifier.ValueText;

                            // enum value as string
                            modelEnum = modelNameSyntax.Name.Identifier.ValueText;
                        }
                    }


                    if(!TypeList.TryGetValue(modelName , out Container val))
                    {
                        val = new Container();
                        val.ModelName = modelName;
                        TypeList.Add(modelName, val);
                    }

                    switch (modelEnum)
                    {
                        case nameof(ClassType.BEHAVIOUR):
                            {
                                val.Behaviour.ModelName = modelName;
                                val.Behaviour.TextAsset = txt;
                                val.Behaviour.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}Behaviour");
                                break;
                            }
                        case nameof(ClassType.DATA):
                            {
                                val.Data.ModelName = modelName;
                                val.Data.TextAsset = txt;
                                val.Data.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}Data");
                                break;
                            }
                        case nameof(ClassType.GAME_DATA):
                            {
                                val.GameData.ModelName = modelName;
                                val.GameData.TextAsset = txt;
                                val.GameData.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}GameData");
                                break;
                            }
                        case nameof(ClassType.INSTANCE):
                            {
                                val.Instance.ModelName = modelName;
                                val.Instance.TextAsset = txt;
                                val.Instance.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}Instance");
                                break;
                            }
                        case nameof(ClassType.STATE):
                            {
                                val.State.ModelName = modelName;
                                val.State.TextAsset = txt;
                                val.State.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}State");
                                break;
                            }
                        case nameof(ClassType.LOAD_SAVE_HANDLER):
                            {
                                val.LoadSaveHandler.ModelName = modelName;
                                val.LoadSaveHandler.TextAsset = txt;
                                val.LoadSaveHandler.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}LoadSaveHandler");
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
        }



    }
}
