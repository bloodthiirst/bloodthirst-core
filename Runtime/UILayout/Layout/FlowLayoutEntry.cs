using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    public static class FlowLayoutEntry
    {
        private static readonly IFlow flowInline = new FlowInline();
        private static readonly IFlow flowBlock = new FlowBlock();
        private static readonly IFlow flowNone = new FlowInline();

        private static readonly IFlow flowInlineRoot = new FlowBlock();

        private static IFlow[] flowEnumLookup = new IFlow[]
        {
            flowInline,
            flowBlock,
            flowNone
        };

        public static void FlowRoot(ILayoutBox layoutBoxRoot)
        {
            FlowContext ctx = new FlowContext();
            ctx.LayoutsWithFlowApplied.Add(layoutBoxRoot);
            flowBlock.Flow(layoutBoxRoot, ctx);
        }

        public static void Flow(ILayoutBox layoutBox, FlowContext context)
        {
            int layoutIndex = (int) layoutBox.LayoutStyle.DisplayType.DisplayKeyword;

            context.LayoutsWithFlowApplied.Add(layoutBox);
            flowEnumLookup[layoutIndex].Flow(layoutBox , context);
        }

        public static void FlowWidth(ILayoutBox layoutBox, FlowContext context)
        {
            int layoutIndex = (int)layoutBox.LayoutStyle.DisplayType.DisplayKeyword;

            context.LayoutsWithFlowApplied.Add(layoutBox);
            flowEnumLookup[layoutIndex].CalculateHeight(layoutBox, context);
        }

        public static void FlowHeight(ILayoutBox layoutBox, FlowContext context)
        {
            int layoutIndex = (int)layoutBox.LayoutStyle.DisplayType.DisplayKeyword;

            context.LayoutsWithFlowApplied.Add(layoutBox);
            flowEnumLookup[layoutIndex].CalculateHeight(layoutBox, context);
        }

        public static void FlowPlacement(ILayoutBox layoutBox, FlowContext context)
        {
            int layoutIndex = (int)layoutBox.LayoutStyle.DisplayType.DisplayKeyword;

            context.LayoutsWithFlowApplied.Add(layoutBox);
            flowEnumLookup[layoutIndex].CalculatePlacement(layoutBox, context);
        }
    }
}
