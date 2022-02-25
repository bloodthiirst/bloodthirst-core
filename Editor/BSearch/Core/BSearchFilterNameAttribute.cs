using System;

namespace Bloodthirst.Editor.BSearch
{
    public class BSearchFilterNameAttribute : Attribute
    {
        public string Name { get; set; }

        public BSearchFilterNameAttribute(string name)
        {
            Name = name;
        }
    }
}