using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    public interface IFlow
    {
        void FlowWidth(ILayoutBox layoutBox, FlowContext context);

        void FlowHeight(ILayoutBox layoutBox, FlowContext context);

        void FlowPlacement(ILayoutBox layoutBox, FlowContext context);
    }
}
