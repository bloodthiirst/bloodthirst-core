using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bloodthirst.Core.Utils
{
    public enum SECTION_EDGE
    {
        START,
        END
    }


    public static class StringExtensions
    {

        private static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        /// <summary>
        /// replace part of the string between "start" and "end"
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="replaceWith"></param>
        /// <returns></returns>
        public static string ReplaceBetween(this string txt, int start, int end, string replaceWith)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(txt, 0, start);
            sb.Append(replaceWith);
            sb.Append(txt, end, txt.Length - end);

            return sb.ToString();
        }


        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        public struct SectionInfo
        {
            public SECTION_EDGE sectionEdge;
            public int startIndex;
            public int endIndex;
        }

        /// <summary>
        /// Given a source text , a start string and an end string , return the start index and end index
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<SectionInfo> StringReplaceSection(this string txt, string start, string end)
        {
            // data mean :
            // tyoe of token
            // int : start of token index
            // int : end of token index
            List<SectionInfo> sectionEdges = new List<SectionInfo>();

            // all starts
            List<int> allStarts = AllIndexesOf(txt, start);

            // all ends
            List<int> allEnds = AllIndexesOf(txt, end);

            foreach (int str in allStarts)
            {
                sectionEdges.Add(new SectionInfo() { sectionEdge = SECTION_EDGE.START, startIndex = str, endIndex = str + start.Length });
            }

            foreach (int str in allEnds)
            {
                sectionEdges.Add(new SectionInfo() { sectionEdge = SECTION_EDGE.END, startIndex = str, endIndex = str + end.Length });
            }

            // order by index in the original text
            sectionEdges = sectionEdges.OrderBy(t => t.startIndex).ToList();

            return sectionEdges;
        }
    }
}
