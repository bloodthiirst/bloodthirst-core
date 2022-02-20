using System;
using System.Text;

namespace Bloodthirst.BJson
{
    public static class BJsonExtensions
    {
        public static void AddIndentation(this StringBuilder sb , int tabCount)
        {
            for (int i = 0; i < tabCount; i++)
            {
                sb.Append('\t');
            }
        }
    }
}