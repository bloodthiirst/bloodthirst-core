using System.Collections.Generic;

namespace Bloodthirst.Core.UILayout
{
    public class FlowContext
    {
        public List<ILayoutBox> LayoutsWithFlowApplied { get; set; } = new List<ILayoutBox>();
    }
}
