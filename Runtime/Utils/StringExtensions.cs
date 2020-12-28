using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bloodthirst.Core.Utils
{
    public static class StringExtensions
    {
        public enum SECTION_EDGE
        {
            START,
            END
        }

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
        public static string ReplaceBetween(this string txt , int start , int end , string replaceWith )
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(txt, 0, start);
            sb.Append(replaceWith);
            sb.Append(txt, end, txt.Length - end);

            return sb.ToString();
        }

        /// <summary>
        /// Given a source text , a start string and an end string , return the start index and end index
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<Tuple<SECTION_EDGE, int, int>> StringReplaceSection(this string txt , string start , string end)
        {
            // data mean :
            // tyoe of token
            // int : start of token index
            // int : end of token index
            List<Tuple<SECTION_EDGE, int , int>> sectionEdges = new List<Tuple<SECTION_EDGE, int ,int>>();

            // all starts
            List<int> allStarts = AllIndexesOf(txt, start);

            // all ends
            List<int> allEnds = AllIndexesOf(txt, end);

            foreach(int str in allStarts)
            {
                sectionEdges.Add(new Tuple<SECTION_EDGE, int, int>(SECTION_EDGE.START, str, str + start.Length));
            }

            foreach (int str in allEnds)
            {
                sectionEdges.Add(new Tuple<SECTION_EDGE, int, int>(SECTION_EDGE.END, str, str + end.Length));
            }

            // order by index in the original text
            sectionEdges = sectionEdges.OrderBy(t => t.Item2).ToList();

            return sectionEdges;
        }
    }
}
