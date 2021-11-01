using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodthirst.Core.EnumLookup
{
    public class EnumLookup<TEnum,TElement> : IReadOnlyList<TElement> where TEnum : Enum
    {
        private static readonly Type type = typeof(TEnum);

        private static readonly Array EnumValues = Enum.GetValues(type);

        private static readonly int EnumCount = EnumValues.Length;

        private List<TElement> elements = new List<TElement>(EnumCount);

        public EnumLookup()
        {
            for(int i = 0; i < EnumCount; i++)
            {
                elements.Add(default(TElement));
            }
        }
        public TElement this[TEnum index] => elements[ (int) ((object)index) ];
        public void Set(TElement index , TElement val)
        {
            elements[(int)((object)index)] = val;
        }
        public int Count => EnumCount;
        public TElement this[int index] => elements[index];
        public IEnumerator<TElement> GetEnumerator() => elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => elements.GetEnumerator();
    }
}
