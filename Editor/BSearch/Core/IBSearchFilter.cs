using System;
using System.Collections.Generic;

namespace Bloodthirst.Editor.BSearch
{
    public interface IBSearchFilter
    {
        event Action<IBSearchFilter> OnFilterChanged;
        bool IsValid();
        List<List<ResultPath>> GetSearchResults(IEnumerable<object> rootIsntancesa);
    }
}