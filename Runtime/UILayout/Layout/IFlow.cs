using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    public interface IFlow
    {
        void Flow(ILayoutBox box , FlowContext context);
        void CalculateWidth(ILayoutBox layoutBox, FlowContext context);

        void CalculateHeight(ILayoutBox layoutBox, FlowContext context);

        void CalculatePlacement(ILayoutBox layoutBox, FlowContext context);
    }
}
