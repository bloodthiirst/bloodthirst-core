using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    public class FlowInline : IFlow
    {
        public void FlowWidth(ILayoutBox layoutBox, FlowContext context)
        {
            // width section
            List<ILayoutBox> autoWidthBoxes = new List<ILayoutBox>();
            float accumulatedWidth = 0;

            //recursing list
            List<ILayoutBox> recursiveLayouts = new List<ILayoutBox>();

            foreach (ILayoutBox childLayout in layoutBox.ChildLayouts)
            {
                bool preFlowWidth = childLayout.LayoutStyle.Width.KeywordValue == UnitKeyword.Content && childLayout.LayoutStyle.Width.UnitType == UnitType.KEYWORD;

                if (!preFlowWidth)
                {
                    recursiveLayouts.Add(childLayout);
                    continue;
                }

                FlowLayoutEntry.FlowWidth(childLayout, context);

                accumulatedWidth += childLayout.Rect.width;
            }

            for (int i = 0; i < layoutBox.ChildLayouts.Count; i++)
            {
                ILayoutBox childLayout = layoutBox.ChildLayouts[i];

                // keyword
                // if width is auto
                // then we get the width of all children
                switch (childLayout.LayoutStyle.Width.UnitType)
                {
                    case UnitType.KEYWORD:
                        {
                            if (childLayout.LayoutStyle.Width.KeywordValue == UnitKeyword.Auto)
                            {
                                autoWidthBoxes.Add(childLayout);
                            }
                            break;
                        }
                    case UnitType.PERCENTAGE:
                        {
                            float w = childLayout.ParentLayout.Rect.width * (childLayout.LayoutStyle.Width.UnitValue / 100f);
                            accumulatedWidth += w;
                            childLayout.Rect.width = w;
                            break;
                        }
                    case UnitType.PIXEL:
                        {
                            float w = childLayout.LayoutStyle.Width.UnitValue;
                            accumulatedWidth += w;
                            childLayout.Rect.width = w;
                            break;
                        }
                }
            }

            // if we have boxes with auto width
            if (autoWidthBoxes.Count != 0)
            {
                // the rest of the width to divide between the auto boxes
                float widthLeft = Mathf.Abs(layoutBox.Rect.width - accumulatedWidth);
                float wPerAuto = widthLeft / autoWidthBoxes.Count;
                foreach (ILayoutBox c in autoWidthBoxes)
                {
                    c.Rect.width = wPerAuto;
                }
            }

            // stretch to content
            if (layoutBox.LayoutStyle.Width.UnitType == UnitType.KEYWORD && layoutBox.LayoutStyle.Width.KeywordValue == UnitKeyword.Content)
            {
                layoutBox.Rect.width = layoutBox.GetChildrenWidthSum();
            }

            foreach(ILayoutBox c in recursiveLayouts)
            {
                FlowLayoutEntry.FlowWidth(c, context);
            }
        }

        public void FlowHeight(ILayoutBox layoutBox, FlowContext context)
        {
            // height section
            List<ILayoutBox> autoHeightBoxes = new List<ILayoutBox>();
            float maxHeight = layoutBox.Rect.height;

            for (int i = 0; i < layoutBox.ChildLayouts.Count; i++)
            {
                ILayoutBox childLayout = layoutBox.ChildLayouts[i];

                // keyword
                // if width is auto
                // then we get the width of all children
                switch (childLayout.LayoutStyle.Height.UnitType)
                {
                    case UnitType.KEYWORD:
                        {
                            if (childLayout.LayoutStyle.Height.KeywordValue == UnitKeyword.Auto)
                            {
                                autoHeightBoxes.Add(childLayout);
                            }
                            break;
                        }
                    case UnitType.PERCENTAGE:
                        {
                            // we do this after we get max height
                            break;
                        }
                    case UnitType.PIXEL:
                        {
                            float h = childLayout.LayoutStyle.Height.UnitValue;
                            maxHeight = Mathf.Max(maxHeight, h);
                            childLayout.Rect.height = h;
                            break;
                        }
                }
            }

            // set percentage height
            for (int i = 0; i < layoutBox.ChildLayouts.Count; i++)
            {
                ILayoutBox childLayout = layoutBox.ChildLayouts[i];

                // keyword
                // if width is auto
                // then we get the width of all children
                switch (childLayout.LayoutStyle.Height.UnitType)
                {
                    case UnitType.PERCENTAGE:
                        {
                            float h = childLayout.ParentLayout.Rect.height * (maxHeight / 100f);
                            maxHeight = Mathf.Max(maxHeight, h);
                            childLayout.Rect.height = h;
                            break;
                        }
                }
            }

            // the rest of the width to divide between the auto boxes
            foreach (ILayoutBox c in autoHeightBoxes)
            {
                c.Rect.height = layoutBox.Rect.height;
            }

            // stretch to content
            if (layoutBox.LayoutStyle.Height.UnitType == UnitType.KEYWORD && layoutBox.LayoutStyle.Height.KeywordValue == UnitKeyword.Content)
            {
                layoutBox.Rect.height = layoutBox.GetChildrenHeightSum();
            }

            foreach (ILayoutBox c in layoutBox.ChildLayouts)
            {
                FlowLayoutEntry.FlowHeight(c, context);
            }
        }

        public void FlowPlacement(ILayoutBox layoutBox, FlowContext context)
        {
            float childOffsetX = 0;

            // parents x offset
            // this is already accumulated due to recursion
            float worldSpaceParentX = layoutBox.Rect.x;
            float worldSpaceParentY = layoutBox.Rect.y;

            // recursivly update the layouts and the position
            foreach (ILayoutBox c in layoutBox.ChildLayouts)
            {
                c.Rect.x = worldSpaceParentX + childOffsetX;
                c.Rect.y = worldSpaceParentY;

                if (!context.LayoutsWithFlowApplied.Contains(c))
                {
                    FlowLayoutEntry.FlowPlacement(c, context);
                    c.PostFlow();
                }


                childOffsetX += c.Rect.width;
            }
        }




    }
}
