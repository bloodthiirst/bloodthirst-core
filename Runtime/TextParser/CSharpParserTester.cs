using Bloodthirst.Core.Tokenizer;
using Bloodthirst.Core.Tokenizer.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSharpParserTester : MonoBehaviour
{
    [SerializeField]
    private TextAsset script;

    // Start is called before the first frame update
    void Start()
    {
        List<TokenBase> tokens = new List<TokenBase>();
        List<TokenProcessorBase<TokenBase>> tokenProcessors = new List<TokenProcessorBase<TokenBase>>()
        {
            // string literal
            new LiteralStringTokenProcessor(),

            // namespace stuff
            new UsingTokenProcessor(),
            new NamespaceTokenProcessor(),

            // access levels
            new PublicTokenProcessor(),
            new PrivateTokenProcessor(),

            // modifiser
            new ConstTokenProcessor(),
            new ReadonlyTokenProcessor(),

            // new
            new NewTokenTokenProcessor(),

            ///// symbols
            // [ ]
            new OpenBracketTokenProcessor(),
            new ClosingBracketTokenProcessor(),

            // { }
            new OpenCurlyBraceTokenProcessor(),
            new ClosingCurlyBraceTokenProcessor(),

            // ( )
            new OpenParenthesisTokenProcessor(),
            new ClosingParenthesisTokenProcessor(),

            // ;
            new SemiColonTokenProcessor(),

            // class
            new ClassTokenProcessor()
        };

        TextTokenizer<TokenBase,CustomIdentifierToken> tokenizer = new TextTokenizer<TokenBase, CustomIdentifierToken>();

        tokenizer.Tokenize(script.text, tokenProcessors, tokens);

        foreach (TokenBase t in tokens)
        {
            Debug.Log($"{t.SourceText}");
        }
    }
}
