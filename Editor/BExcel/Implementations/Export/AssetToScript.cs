using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Bloodthirst.Editor.BExcelEditor
{
    public class AssetToScript
    {
        public static string Convert(BExcelOutput excel)
        {
            List<SyntaxNodeOrToken> languagesTokens = new List<SyntaxNodeOrToken>();
            {
                foreach (var lan in excel.languages)
                {
                    var elem = EnumMemberDeclaration(
                        Identifier(TriviaList(Whitespace("            ")),
                        lan,
                        TriviaList()));

                    var spacing = Token(
                        TriviaList(),
                        SyntaxKind.CommaToken,
                        TriviaList(CarriageReturnLineFeed));

                    languagesTokens.Add(elem);
                    languagesTokens.Add(spacing);
                }
            }

            List<SyntaxNodeOrToken> idsTokens = new List<SyntaxNodeOrToken>();
            {
                foreach (var tab in excel.tabs)
                {
                    foreach (var row in tab.rows)
                    {
                        var elem = EnumMemberDeclaration(
                            Identifier(TriviaList(Whitespace("            ")),
                            row.key,
                            TriviaList()));

                        var spacing = Token(
                        TriviaList(),
                        SyntaxKind.CommaToken,
                        TriviaList(CarriageReturnLineFeed));

                        idsTokens.Add(elem);
                        idsTokens.Add(spacing);
                    }
                }
            }

            List<SyntaxNodeOrToken> kvEntries = new List<SyntaxNodeOrToken>();
            List<SyntaxNodeOrToken> entriesArray = new List<SyntaxNodeOrToken>();
            foreach (var tab in excel.tabs)
            {
                foreach (var row in tab.rows)
                {
                    entriesArray.Clear();
                    foreach (var e in row.entries)
                    {
                        entriesArray.Add(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                                        Literal(
                                                                                            TriviaList(),
                                                                                            $"\"{e}\"",
                                                                                            e,
                                                                                            TriviaList(
                                                                                                Space))));

                        entriesArray.Add(Token(TriviaList(),
                                                                                        SyntaxKind.CommaToken,
                                                                                        TriviaList(
                                                                                            Space)));
                    }

                    // kv expression
                    var kvExp = InitializerExpression(
                                                            SyntaxKind.ComplexElementInitializerExpression,
                                                            SeparatedList<ExpressionSyntax>(
                                                                new SyntaxNodeOrToken[]{
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        IdentifierName("LocalizationID"),
                                                                        IdentifierName(
                                                                            Identifier(
                                                                                TriviaList(),
                                                                                row.key,
                                                                                TriviaList(
                                                                                    Space)))),
                                                                    Token(
                                                                        TriviaList(),
                                                                        SyntaxKind.CommaToken,
                                                                        TriviaList(
                                                                            Space)),
                                                                    ArrayCreationExpression(
                                                                        ArrayType(
                                                                            PredefinedType(
                                                                                Token(SyntaxKind.StringKeyword)))
                                                                        .WithRankSpecifiers(
                                                                            SingletonList<ArrayRankSpecifierSyntax>(
                                                                                ArrayRankSpecifier(
                                                                                    SingletonSeparatedList<ExpressionSyntax>(
                                                                                        OmittedArraySizeExpression()))
                                                                                .WithCloseBracketToken(
                                                                                    Token(
                                                                                        TriviaList(),
                                                                                        SyntaxKind.CloseBracketToken,
                                                                                        TriviaList(
                                                                                            Space))))))
                                                                    .WithNewKeyword(
                                                                        Token(
                                                                            TriviaList(),
                                                                            SyntaxKind.NewKeyword,
                                                                            TriviaList(
                                                                                Space)))
                                                                    .WithInitializer(
                                                                        InitializerExpression(
                                                                            SyntaxKind.ArrayInitializerExpression,
                                                                            SeparatedList<ExpressionSyntax>(

                                                                                entriesArray


                                                                                ))
                                                                        .WithOpenBraceToken(
                                                                            Token(
                                                                                TriviaList(),
                                                                                SyntaxKind.OpenBraceToken,
                                                                                TriviaList(
                                                                                    Space))))}));
                    
                    // add braces
                    kvExp = kvExp.WithOpenBraceToken(Token(
                                                                TriviaList(
                                                                    Whitespace("            ")),
                                                                SyntaxKind.OpenBraceToken,
                                                                TriviaList(
                                                                    Space)));
                    // command and new line
                    SyntaxToken commaEndLine = Token(TriviaList(), SyntaxKind.CommaToken, TriviaList(LineFeed));

                    kvEntries.Add(kvExp);
                    kvEntries.Add(commaEndLine);
                }
            }


            List<SyntaxNodeOrToken> valueEntries = new List<SyntaxNodeOrToken>();
            {
                foreach (var tab in excel.tabs)
                {
                    foreach (var row in tab.rows)
                    {
                        for (int i = 0; i < row.entries.Length; i++)
                        {
                            string entry = row.entries[i];

                            SyntaxTriviaList trivia = default;
                            if (i == 0)
                            {
                                trivia = TriviaList(
                                    Whitespace("            "),
                                    Comment($"// {row.key}"),
                                    CarriageReturnLineFeed,
                                    Whitespace("            "));
                            }
                            else
                            {
                                trivia = TriviaList(Whitespace(" "));
                            }

                            var elem = LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Literal(trivia,
                                    "\"" + entry + "\"",
                                    entry,
                                    TriviaList())
                                );

                            valueEntries.Add(elem);

                            if (i != row.entries.Length - 1)
                            {
                                var spacing = Token(
                                    TriviaList(),
                                    SyntaxKind.CommaToken,
                                    TriviaList(new[] { Tab }));

                                valueEntries.Add(spacing);
                            }
                            else
                            {
                                var spacing = Token(
                                    TriviaList(),
                                    SyntaxKind.CommaToken,
                                    TriviaList(new[] { Space, CarriageReturnLineFeed, CarriageReturnLineFeed }));

                                valueEntries.Add(spacing);
                            }
                        }
                    }
                }
            }

            CompilationUnitSyntax compilationUnit = CompilationUnit()
    .WithUsings
    (
    SingletonList<UsingDirectiveSyntax>
    (
        UsingDirective
        (
            QualifiedName
            (
                QualifiedName
                (
                    IdentifierName("System"),
                    IdentifierName("Collections")
                ),
                IdentifierName("Generic")
            )
        )
        .WithUsingKeyword
        (
            Token
            (
                TriviaList
                (
                    Whitespace("    ")
                ),
                SyntaxKind.UsingKeyword,
                TriviaList
                (
                    Space
                )
            )
        )
        .WithSemicolonToken
        (
            Token
            (
                TriviaList(),
                SyntaxKind.SemicolonToken,
                TriviaList
                (
                    LineFeed
                )
            )
        )
    )
)
    .WithMembers(
    SingletonList<MemberDeclarationSyntax>(
        
        // START NAMESPACE
        NamespaceDeclaration(ParseName(excel.scriptNamespace))
        .WithNamespaceKeyword(
            Token(
                TriviaList(),
                SyntaxKind.NamespaceKeyword,
                TriviaList(
                    Space)))
        .WithOpenBraceToken(
            Token(
                TriviaList(),
                SyntaxKind.OpenBraceToken,
                TriviaList(
                    CarriageReturnLineFeed)))
        // END NAMESPACE
        
        .WithMembers(
            SingletonList<MemberDeclarationSyntax>(
                ClassDeclaration(
                    Identifier(
                        TriviaList(),
                        "LocalizationSource",
                        TriviaList(
                            CarriageReturnLineFeed)))
                .WithModifiers(
                    TokenList(
                        new[]{
                            Token(
                                TriviaList(
                                    Whitespace("    ")),
                                SyntaxKind.PublicKeyword,
                                TriviaList(
                                    Space)),
                            Token(
                                TriviaList(),
                                SyntaxKind.StaticKeyword,
                                TriviaList(
                                    Space)),
                        Token(
                                TriviaList(),
                                SyntaxKind.PartialKeyword,
                                TriviaList(
                                    Space))}))
                .WithKeyword(
                    Token(
                        TriviaList(),
                        SyntaxKind.ClassKeyword,
                        TriviaList(
                            Space)))
                .WithOpenBraceToken(
                    Token(
                        TriviaList(
                            Whitespace("    ")),
                        SyntaxKind.OpenBraceToken,
                        TriviaList(
                            CarriageReturnLineFeed)))
                .WithMembers(
                    List<MemberDeclarationSyntax>(
                        new MemberDeclarationSyntax[]{
                            EnumDeclaration(
                                Identifier(
                                    TriviaList(),
                                    "Language",
                                    TriviaList(
                                        CarriageReturnLineFeed)))
                            .WithModifiers(
                                TokenList(
                                    Token(
                                        TriviaList(
                                            Whitespace("        ")),
                                        SyntaxKind.PublicKeyword,
                                        TriviaList(
                                            Space))))
                            .WithEnumKeyword(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.EnumKeyword,
                                    TriviaList(
                                        Space)))
                            .WithOpenBraceToken(
                                Token(
                                    TriviaList(
                                        Whitespace("        ")),
                                    SyntaxKind.OpenBraceToken,
                                    TriviaList(
                                        CarriageReturnLineFeed)))
                            .WithMembers(
                                SeparatedList<EnumMemberDeclarationSyntax>(
                                    languagesTokens))
                            .WithCloseBraceToken(
                                Token(
                                    TriviaList(
                                        Whitespace("        ")),
                                    SyntaxKind.CloseBraceToken,
                                    TriviaList(
                                        CarriageReturnLineFeed))),
                            EnumDeclaration(
                                Identifier(
                                    TriviaList(),
                                    "LocalizationID",
                                    TriviaList(
                                        CarriageReturnLineFeed)))
                            .WithModifiers(
                                TokenList(
                                    Token(
                                        TriviaList(
                                            new []{
                                                CarriageReturnLineFeed,
                                                Whitespace("        ")}),
                                        SyntaxKind.PublicKeyword,
                                        TriviaList(
                                            Space))))
                            .WithEnumKeyword(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.EnumKeyword,
                                    TriviaList(
                                        Space)))
                            .WithOpenBraceToken(
                                Token(
                                    TriviaList(
                                        Whitespace("        ")),
                                    SyntaxKind.OpenBraceToken,
                                    TriviaList(
                                        CarriageReturnLineFeed)))
                            .WithMembers(
                                SeparatedList<EnumMemberDeclarationSyntax>(
                                    idsTokens))
                            .WithCloseBraceToken(
                                Token(
                                    TriviaList(
                                        Whitespace("        ")),
                                    SyntaxKind.CloseBraceToken,
                                    TriviaList(
                                        CarriageReturnLineFeed))),
                                                
                            
                            FieldDeclaration
                    (
                        VariableDeclaration
                        (
                            GenericName
                            (
                                Identifier("Dictionary")
                            )
                            .WithTypeArgumentList
                            (
                                TypeArgumentList
                                (
                                    SeparatedList<TypeSyntax>
                                    (
                                        new SyntaxNodeOrToken[]
                                        {
                                            IdentifierName("LocalizationID"),
                                            Token
                                            (
                                                TriviaList(),
                                                SyntaxKind.CommaToken,
                                                TriviaList
                                                (
                                                    Space
                                                )
                                            ),
                                            ArrayType
                                            (
                                                PredefinedType
                                                (
                                                    Token(SyntaxKind.StringKeyword)
                                                )
                                            )
                                            .WithRankSpecifiers
                                            (
                                                SingletonList<ArrayRankSpecifierSyntax>
                                                (
                                                    ArrayRankSpecifier
                                                    (
                                                        SingletonSeparatedList<ExpressionSyntax>
                                                        (
                                                            OmittedArraySizeExpression()
                                                        )
                                                    )
                                                )
                                            )
                                        }
                                    )
                                )
                                .WithGreaterThanToken
                                (
                                    Token
                                    (
                                        TriviaList(),
                                        SyntaxKind.GreaterThanToken,
                                        TriviaList
                                        (
                                            Space
                                        )
                                    )
                                )
                            )
                        )
                        .WithVariables
                        (
                            SingletonSeparatedList<VariableDeclaratorSyntax>
                            (
                                VariableDeclarator
                                (
                                    Identifier
                                    (
                                        TriviaList(),
                                        "lookupDict",
                                        TriviaList
                                        (
                                            Space
                                        )
                                    )
                                )
                                .WithInitializer
                                (
                                    EqualsValueClause
                                    (
                                        ObjectCreationExpression
                                        (
                                            GenericName
                                            (
                                                Identifier("Dictionary")
                                            )
                                            .WithTypeArgumentList
                                            (
                                                TypeArgumentList
                                                (
                                                    SeparatedList<TypeSyntax>
                                                    (
                                                        new SyntaxNodeOrToken[]
                                                        {
                                                            IdentifierName("LocalizationID"),
                                                            Token
                                                            (
                                                                TriviaList(),
                                                                SyntaxKind.CommaToken,
                                                                TriviaList
                                                                (
                                                                    Space
                                                                )
                                                            ),
                                                            ArrayType
                                                            (
                                                                PredefinedType
                                                                (
                                                                    Token(SyntaxKind.StringKeyword)
                                                                )
                                                            )
                                                            .WithRankSpecifiers
                                                            (
                                                                SingletonList<ArrayRankSpecifierSyntax>
                                                                (
                                                                    ArrayRankSpecifier
                                                                    (
                                                                        SingletonSeparatedList<ExpressionSyntax>
                                                                        (
                                                                            OmittedArraySizeExpression()
                                                                        )
                                                                    )
                                                                )
                                                            )
                                                        }
                                                    )
                                                )
                                            )
                                        )
                                        .WithNewKeyword
                                        (
                                            Token
                                            (
                                                TriviaList(),
                                                SyntaxKind.NewKeyword,
                                                TriviaList
                                                (
                                                    Space
                                                )
                                            )
                                        )
                                        .WithArgumentList
                                        (
                                            ArgumentList()
                                            .WithCloseParenToken
                                            (
                                                Token
                                                (
                                                    TriviaList(),
                                                    SyntaxKind.CloseParenToken,
                                                    TriviaList
                                                    (
                                                        LineFeed
                                                    )
                                                )
                                            )
                                        )
                                        .WithInitializer
                                        (
                                            InitializerExpression
                                            (
                                                SyntaxKind.CollectionInitializerExpression,
                                                SeparatedList<ExpressionSyntax>
                                                (
                                                    kvEntries
                                                )
                                            )
                                            .WithOpenBraceToken
                                            (
                                                Token
                                                (
                                                    TriviaList
                                                    (
                                                        Whitespace("        ")
                                                    ),
                                                    SyntaxKind.OpenBraceToken,
                                                    TriviaList
                                                    (
                                                        LineFeed
                                                    )
                                                )
                                            )
                                            .WithCloseBraceToken
                                            (
                                                Token
                                                (
                                                    TriviaList
                                                    (
                                                        Whitespace("        ")
                                                    ),
                                                    SyntaxKind.CloseBraceToken,
                                                    TriviaList
                                                    (
                                                        LineFeed
                                                    )
                                                )
                                            )
                                        )
                                    )
                                    .WithEqualsToken
                                    (
                                        Token
                                        (
                                            TriviaList(),
                                            SyntaxKind.EqualsToken,
                                            TriviaList
                                            (
                                                Space
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                            .WithModifiers(
                                TokenList(
                                    new []{
                                        Token(
                                            TriviaList(
                                                new []{
                                                    Whitespace("        "),
                                                    CarriageReturnLineFeed,
                                                    Whitespace("        ")}),
                                            SyntaxKind.PrivateKeyword,
                                            TriviaList(
                                                Space)),
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.StaticKeyword,
                                            TriviaList(
                                                Space)),
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.ReadOnlyKeyword,
                                            TriviaList(
                                                Space))}))
                            .WithSemicolonToken(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.SemicolonToken,
                                    TriviaList(
                                        CarriageReturnLineFeed)))
                            }))
                .WithCloseBraceToken(
                    Token(
                        TriviaList(
                            Whitespace("    ")),
                        SyntaxKind.CloseBraceToken,
                        TriviaList(
                            CarriageReturnLineFeed)))))));

            var code = compilationUnit
                .ToFullString();

            return code;
        }
    }
}