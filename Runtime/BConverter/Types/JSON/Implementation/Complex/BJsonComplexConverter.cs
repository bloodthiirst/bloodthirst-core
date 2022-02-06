using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.BDeepCopy
{
    internal class BJsonComplexConverter : BJsonComplexBaseConverter
    {
        private BTypeData TypeData { get; set; }

        private Dictionary<string, BMemberData> MemberToDataDictionary { get; set; }

        public BJsonComplexConverter(Type t) : base(t)
        {
            TypeData = BTypeProvider.GetOrCreate(t);
            MemberToDataDictionary = TypeData.MemberDatas.ToDictionary(k => k.MemberInfo.Name, k => k);
        }

        public override void Initialize()
        {
            
        }
        public override object CreateInstance_Internal()
        {
            return TypeData.Constructor();
        }

        public override string To_Internal(object instance, BConverterContext context, BConverterSettings settings)
        {
            throw new NotImplementedException();
        }

        public override object From_Internal(object instance, ref ParserState<JSONTokenType> parseState, BConverterContext context, BConverterSettings settings)
        {
            // skip the first object start
            parseState.CurrentTokenIndex++;

            while (parseState.CurrentTokenIndex < parseState.Tokens.Count)
            {
                Token<JSONTokenType> currentToken = parseState.Tokens[parseState.CurrentTokenIndex];

                // skip spaces
                if (currentToken.TokenType == JSONTokenType.SPACE)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // exit if object ended
                if (currentToken.TokenType == JSONTokenType.OBJECT_END)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // stp while no identitifer
                if (currentToken.TokenType != JSONTokenType.IDENTIFIER)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // key found
                string key = currentToken.AsString();

                // skip until the first colon
                parseState.SkipUntil(IsColon);

                parseState.CurrentTokenIndex++;
                currentToken = parseState.Tokens[parseState.CurrentTokenIndex];

                // skip space
                parseState.SkipWhile(IsSkippableSpace);

                if (MemberToDataDictionary.TryGetValue(key, out BMemberData memData))
                {
                    Debug.Log($"Member found  {key} of type { TypeUtils.GetNiceName(memData.Type) }");

                    IBJsonConverterInternal c = (IBJsonConverterInternal)Provider.Get(memData.Type);

                    object oldVal = instance == null ? null : memData.MemberGetter(instance);
                    object newVal = c.From_Internal(oldVal, ref parseState, context, settings);

                    memData.MemberSetter(instance, newVal);
                }

                // skip until the first comma or object end
                parseState.SkipUntil(IsPropertyEnd);

                if(parseState.CurrentToken.TokenType == JSONTokenType.OBJECT_END)
                {
                    parseState.CurrentTokenIndex++;
                    break;
                }
            }

            return instance;
        }

        #region utils
        private static bool IsPropertyEnd(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.OBJECT_END || token.TokenType == JSONTokenType.COMMA;
        }

        private static bool IsSkippableSpace(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.SPACE || token.TokenType == JSONTokenType.NEW_LINE;
        }

        private static bool IsComma(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.COMMA;
        }

        private static bool IsColon(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.COLON;
        }

        #endregion


    }
}
