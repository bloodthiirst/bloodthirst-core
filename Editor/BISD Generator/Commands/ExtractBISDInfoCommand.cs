using Bloodthirst.Core.BISD.CodeGeneration;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
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
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.BISD.Editor.Commands
{
    public class ExtractBISDInfoCommand : CommandInstant<ExtractBISDInfoCommand, Dictionary<string, BISDInfoContainer>>
    {
        public ExtractBISDInfoCommand()
        {
        }

        protected override Dictionary<string, BISDInfoContainer> GetResult()
        {
            return ExtractBISDInfo();
        }


        /// <summary>
        /// Extracts info about the classes that follow the BISD pattern
        /// </summary>
        /// <param name="TypeList"></param>
        /// <param name="TextList"></param>
        private static Dictionary<string, BISDInfoContainer> ExtractBISDInfo()
        {
            Dictionary<string, BISDInfoContainer> typeList = new Dictionary<string, BISDInfoContainer>();

            IReadOnlyList<Type> allTypes = TypeUtils.AllTypes;

            typeList = new Dictionary<string, BISDInfoContainer>();

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

                    string fileFolder = AssetDatabase.GetAssetPath(txt);

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
                                val.Behaviour.TextAsset = txt;
                                val.Behaviour.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}Behaviour");
                                break;
                            }
                        case nameof(ClassType.DATA):
                            {
                                val.Data = new BISDInfo() { Container = val };

                                val.Data.ModelName = modelName;
                                val.Data.TextAsset = txt;
                                val.Data.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}Data");
                                break;
                            }
                        case nameof(ClassType.GAME_DATA):
                            {
                                val.GameData = new BISDInfo() { Container = val };

                                val.GameData.ModelName = modelName;
                                val.GameData.TextAsset = txt;
                                val.GameData.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}GameData");
                                break;
                            }
                        case nameof(ClassType.INSTANCE):
                            {
                                val.Instance = new BISDInfo() { Container = val };

                                val.Instance.ModelName = modelName;
                                val.Instance.TextAsset = txt;
                                val.Instance.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}Instance");
                                break;
                            }
                        case nameof(ClassType.STATE):
                            {
                                val.State = new BISDInfo() { Container = val };

                                val.State.ModelName = modelName;
                                val.State.TextAsset = txt;
                                val.State.TypeRef = allTypes.FirstOrDefault(t => t.Name == $"{modelName}State");
                                break;
                            }
                        case nameof(ClassType.LOAD_SAVE_HANDLER):
                            {
                                val.LoadSaveHandler = new BISDInfo() { Container = val };

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

            return typeList;
        }

    }
}
