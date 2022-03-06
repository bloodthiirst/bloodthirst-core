using System;

namespace Bloodthirst.Editor.BSearch
{
    public class BExcelFilterNameAttribute : Attribute
    {
        public string Name { get; set; }

        public BExcelFilterNameAttribute(string name)
        {
            Name = name;
        }
    }
}