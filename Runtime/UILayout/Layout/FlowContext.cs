using System.Collections.Generic;

namespace Bloodthirst.Core.UILayout
{
    public class FlowContext
    {
        public HashSet<ILayoutBox> FlowWidthCache { get; set; } = new HashSet<ILayoutBox>();
        public HashSet<ILayoutBox> FlowHeightCache { get; set; } = new HashSet<ILayoutBox>();
        public HashSet<ILayoutBox> FlowPlacementCache { get; set; } = new HashSet<ILayoutBox>();
    }
}