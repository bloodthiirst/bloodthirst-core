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

            ctx.FlowWidthCache.Add(layoutBoxRoot);
            flowBlock.FlowWidth(layoutBoxRoot, ctx);

            ctx.FlowHeightCache.Add(layoutBoxRoot);
            flowBlock.FlowHeight(layoutBoxRoot, ctx);

            ctx.FlowPlacementCache.Add(layoutBoxRoot);
            flowBlock.FlowPlacement(layoutBoxRoot, ctx);

            PostFlowRecursive(layoutBoxRoot);
        }

        private static void PostFlowRecursive(ILayoutBox layoutBox)
        {
            layoutBox.PostFlow();

            foreach (ILayoutBox c in layoutBox.ChildLayouts)
            {
                PostFlowRecursive(c);
            }
        }

        public static void FlowWidth(ILayoutBox layoutBox, FlowContext context)
        {
            if (layoutBox == null)
                return;

            if (!context.FlowWidthCache.Add(layoutBox))
                return;

            int layoutIndex = (int)layoutBox.LayoutStyle.DisplayType.DisplayKeyword;

            flowEnumLookup[layoutIndex].FlowWidth(layoutBox, context);
        }

        public static void FlowHeight(ILayoutBox layoutBox, FlowContext context)
        {
            if (layoutBox == null)
                return;

            if (!context.FlowHeightCache.Add(layoutBox))
                return;

            int layoutIndex = (int)layoutBox.LayoutStyle.DisplayType.DisplayKeyword;

            flowEnumLookup[layoutIndex].FlowHeight(layoutBox, context);
        }

        public static void FlowPlacement(ILayoutBox layoutBox, FlowContext context)
        {
            if (layoutBox == null)
                return;

            if (!context.FlowPlacementCache.Add(layoutBox))
                return;

            int layoutIndex = (int)layoutBox.LayoutStyle.DisplayType.DisplayKeyword;

            flowEnumLookup[layoutIndex].FlowPlacement(layoutBox, context);
        }
    }
}
