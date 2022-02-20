using System;
using System.Diagnostics;

namespace Bloodthirst.BJson
{
    [DebuggerDisplay("{ToString(),nq}")]
    public struct Token<TTokenType> where TTokenType : Enum
    {
        public string Text { get; set; }
        public int StartIndex { get; set; }
        public int Length { get; set; }
        public TTokenType TokenType { get; set; }

        public override string ToString()
        {
            return Text.Substring(StartIndex, Length);
        }
    }
}