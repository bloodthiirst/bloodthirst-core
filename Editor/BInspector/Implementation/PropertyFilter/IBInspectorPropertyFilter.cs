using Bloodthirst.BType;
using System;

namespace Bloodthirst.Editor.BInspector
{
    internal interface IBInspectorPropertyFilter
    {
        bool CanFilter(Type t);
        BTypeData FilteredProperties(BTypeData typeData);
    }

}
