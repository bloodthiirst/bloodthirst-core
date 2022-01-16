using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodthirst.Core.EnumLookup
{
    public class EnumLookup<TEnum, TElement> : IReadOnlyList<TElement> where TEnum : Enum
    {
        private List<TElement> elements;

        public EnumLookup()
        {
            elements = new List<TElement>(EnumUtils<TEnum>.EnumCount);

            for(int i = 0; i < EnumUtils<TEnum>.EnumCount; i++)
            {
                elements[i] = default;
            }
        }
        public TElement this[TEnum index]
        {
            get
            {
                return elements[EnumUtils<TEnum>.GetIndex(index)];
            }
            set
            {
                elements[EnumUtils<TEnum>.GetIndex(index)] = value;
            }
        }

        public TElement this[int index]
        {
            get
            {
                return elements[index];
            }
            set
            {
                elements[index] = value;
            }
        }

        public void Set(int index, TElement val)
        {
            elements[index] = val;
        }
        public int Count => EnumUtils<TEnum>.EnumCount;
        public IEnumerator<TElement> GetEnumerator() => elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => elements.GetEnumerator();
    }
}
