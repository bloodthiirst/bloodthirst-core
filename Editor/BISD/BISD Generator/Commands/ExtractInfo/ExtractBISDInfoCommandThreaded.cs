using Bloodthirst.Core.BISD.CodeGeneration;
using Bloodthirst.Core.Utils;
using Bloodthirst.System.CommandSystem;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CSharp;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.BISD.Editor.Commands
{
    public class ExtractBISDInfoCommandThreaded : CommandBase<ExtractBISDInfoCommandThreaded, Dictionary<string, BISDInfoContainer>>
    {
        private static string pathToProject = EditorUtils.PathToProject;

        private struct TextAssetPathPair
        {
            public TextAsset TextAsset { get; set; }
            public string AssetPath { get; set; }
            public string TextContent { get; set; }
        }

        public ExtractBISDInfoCommandThreaded()
        {
        }

        public override void OnStart()
        {
            Result = null;

            List<TextAssetPathPair> deps = FetchUnityAssets().ToList();

            ThreadPool.QueueUserWorkItem( _ =>
            {
                Result = ExtractBISDInfoThreaded(deps);
            });
        }

        private IEnumerable<TextAssetPathPair> FetchUnityAssets()
        {
            List<MonoScript> list = EditorUtils.FindScriptAssets();
            
            for (int i = 0; i < list.Count; i++)
            {
                TextAsset t = list[i];
                yield return new TextAssetPathPair() { TextAsset = t, AssetPath = AssetDatabase.GetAssetPath(t) , TextContent = t.text };
            }
        }

        public override void OnTick(float delta)
        {
            if (Result == null)
                return;

            Success();
        }


        /// <summary>
        /// Extracts info about the classes that follow the BISD pattern
        /// </summary>
        /// <param name="TypeList"></param>
        /// <param name="TextList"></param>
        private Dictionary<string, BISDInfoContainer> ExtractBISDInfoThreaded(List<TextAssetPathPair> deps)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();

            try
            {
                Dictionary<string, BISDInfoContainer> typeList = new Dictionary<string, BISDInfoContainer>();

                IReadOnlyList<Type> allTypes = TypeUtils.AllTypes;

                typeList = new Dictionary<string, BISDInfoContainer>();



                Dictionary<TextAsset, Type> fileToType = new Dictionary<TextAsset, Type>();

                foreach (TextAssetPathPair d in deps)
                {
                    string relativePath = d.AssetPath;

                    string systemPath = pathToProject + relativePath;

                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(d.TextContent);

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

                        string fileFolder = d.AssetPath;

                        fileFolder = Path.GetDirectoryName(fileFolder);
                        fileFolder = fileFolder.Replace(@"\", @"/");

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


                        if (!typeList.TryGetValue(modelName, out BISDInfoContainer val))
                        {
                            val = new BISDInfoContainer();
                            val.ModelName = modelName;
                            val.ModelFolder = fileFolder;
                            typeList.Add(modelName, val);
                        }

                        switch (modelEnum)
                        {
                            case nameof(ClassType.BEHAVIOUR):
                                {
                                    val.Behaviour = new BISDInfo() { Container = val };

                                    val.Behaviour.ModelName = modelName;
                                    val.Behaviour.TextAsset = d.TextAsset;
                                    val.Behaviour.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}Behaviour");
                                    break;
                                }
                            case nameof(ClassType.DATA):
                                {
                                    val.Data = new BISDInfo() { Container = val };

                                    val.Data.ModelName = modelName;
                                    val.Data.TextAsset = d.TextAsset;
                                    val.Data.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}Data");
                                    break;
                                }
                            case nameof(ClassType.GAME_SAVE):
                                {
                                    val.GameSave = new BISDInfo() { Container = val };

                                    val.GameSave.ModelName = modelName;
                                    val.GameSave.TextAsset = d.TextAsset;
                                    val.GameSave.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}GameSave");
                                    break;
                                }
                            case nameof(ClassType.INSTANCE_MAIN):
                                {
                                    val.InstanceMain = new BISDInfo() { Container = val };

                                    val.InstanceMain.ModelName = modelName;
                                    val.InstanceMain.TextAsset = d.TextAsset;
                                    val.InstanceMain.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}Instance");
                                    break;
                                }
                            case nameof(ClassType.INSTANCE_PARTIAL):
                                {
                                    val.InstancePartial = new BISDInfo() { Container = val };

                                    val.InstancePartial.ModelName = modelName;
                                    val.InstancePartial.TextAsset = d.TextAsset;
                                    val.InstancePartial.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}Instance");
                                    break;
                                }
                            case nameof(ClassType.STATE):
                                {
                                    val.State = new BISDInfo() { Container = val };

                                    val.State.ModelName = modelName;
                                    val.State.TextAsset = d.TextAsset;
                                    val.State.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}State");
                                    break;
                                }
                            case nameof(ClassType.GAME_SAVE_HANDLER):
                                {
                                    val.GameSaveHandler = new BISDInfo() { Container = val };

                                    val.GameSaveHandler.ModelName = modelName;
                                    val.GameSaveHandler.TextAsset = d.TextAsset;
                                    val.GameSaveHandler.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}GameSaveHandler");
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }

                return typeList;
            }

            catch(Exception ex)
            {
                Debug.Log($"An exception occured during the threaded scripts scanning :\n{ex.Message}");
            }
            finally
            {
                provider.Dispose();
            }

            return null;
            
        }

    }
}
