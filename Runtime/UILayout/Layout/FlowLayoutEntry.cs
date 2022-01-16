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

            ctx.LayoutsWithFlowApplied.Clear();
            flowBlock.FlowWidth(layoutBoxRoot, ctx);

            ctx.LayoutsWithFlowApplied.Clear();
            flowBlock.FlowHeight(layoutBoxRoot, ctx);

            ctx.LayoutsWithFlowApplied.Clear();
            flowBlock.FlowPlacement(layoutBoxRoot, ctx);
        }

        public static void FlowWidth(ILayoutBox layoutBox, FlowContext context)
        {
            int layoutIndex = (int)layoutBox.LayoutStyle.DisplayType.DisplayKeyword;

            context.LayoutsWithFlowApplied.Add(layoutBox);
            flowEnumLookup[layoutIndex].FlowWidth(layoutBox, context);
        }

        public static void FlowHeight(ILayoutBox layoutBox, FlowContext context)
        {
            int layoutIndex = (int)layoutBox.LayoutStyle.DisplayType.DisplayKeyword;

            context.LayoutsWithFlowApplied.Add(layoutBox);
            flowEnumLookup[layoutIndex].FlowHeight(layoutBox, context);
        }

        public static void FlowPlacement(ILayoutBox layoutBox, FlowContext context)
        {
            int layoutIndex = (int)layoutBox.LayoutStyle.DisplayType.DisplayKeyword;

            context.LayoutsWithFlowApplied.Add(layoutBox);
            flowEnumLookup[layoutIndex].FlowPlacement(layoutBox, context);
        }
    }
}
