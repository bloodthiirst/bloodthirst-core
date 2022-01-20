using System;
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
            FlowLayoutEntry.FlowWidth(layoutBox.ParentLayout, context);

            // width section
            List<ILayoutBox> autoWidthBoxes = new List<ILayoutBox>();
            float accumulatedWidth = 0;

            //recursing list
            List<ILayoutBox> recursiveLayouts = new List<ILayoutBox>();

            // preflow the content base width boxes
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

            // treat display mode position
            for (int i = 0; i < layoutBox.ChildLayouts.Count; i++)
            {
                ILayoutBox childLayout = layoutBox.ChildLayouts[i];

                float w = 0;

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
                                break;
                            }

                            if (childLayout.LayoutStyle.Width.KeywordValue == UnitKeyword.BasedOnHeight)
                            {
                                FlowLayoutEntry.FlowHeight(childLayout, context);
                                childLayout.Rect.width = childLayout.Rect.height * childLayout.LayoutStyle.Width.UnitValue * 0.01f;
                                w = childLayout.Rect.width;
                                break;
                            }

                            break;
                        }
                    case UnitType.PERCENTAGE:
                        {
                            w = childLayout.ParentLayout.Rect.width * (childLayout.LayoutStyle.Width.UnitValue / 100f);
                            childLayout.Rect.width = w;
                            break;
                        }
                    case UnitType.PIXEL:
                        {
                            w = childLayout.LayoutStyle.Width.UnitValue;
                            childLayout.Rect.width = w;
                            break;
                        }
                }

                switch (childLayout.LayoutStyle.PositionType.PositionKeyword)
                {
                    case PositionKeyword.DISPLAY_MODE:
                        {
                            accumulatedWidth += w;
                            break;
                        }
                    default:
                        break;
                }
            }

            // the rest of the width to divide between the auto boxes
            float widthLeft = Mathf.Abs(layoutBox.Rect.width - accumulatedWidth);
            float wPerAuto = widthLeft / autoWidthBoxes.Count;
            foreach (ILayoutBox c in autoWidthBoxes)
            {
                c.Rect.width = wPerAuto;
            }

            // stretch to content
            if (layoutBox.LayoutStyle.Width.UnitType == UnitType.KEYWORD && layoutBox.LayoutStyle.Width.KeywordValue == UnitKeyword.Content)
            {
                layoutBox.Rect.width = layoutBox.GetChildrenWidthSum();
            }

            foreach (ILayoutBox c in recursiveLayouts)
            {
                FlowLayoutEntry.FlowWidth(c, context);
            }
        }

        public void FlowHeight(ILayoutBox layoutBox, FlowContext context)
        {
            FlowLayoutEntry.FlowHeight(layoutBox.ParentLayout, context);

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
                                break;
                            }

                            if (childLayout.LayoutStyle.Height.KeywordValue == UnitKeyword.BasedOnWidth)
                            {
                                FlowLayoutEntry.FlowWidth(childLayout, context);
                                childLayout.Rect.height = childLayout.Rect.width * childLayout.LayoutStyle.Height.UnitValue * 0.01f;
                                break;
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
                switch (c.LayoutStyle.PositionType.PositionKeyword)
                {
                    case PositionKeyword.DISPLAY_MODE:
                        {
                            c.Rect.x = worldSpaceParentX + childOffsetX;
                            c.Rect.y = worldSpaceParentY;

                            if (!context.FlowPlacementCache.Contains(c))
                            {
                                FlowLayoutEntry.FlowPlacement(c, context);
                            }

                            childOffsetX += c.Rect.width;

                            break;
                        }
                    case PositionKeyword.PARENT_SPACE:
                        {
                            c.Rect.x = worldSpaceParentX + c.LayoutStyle.PositionType.PositionValue.x;
                            c.Rect.y = worldSpaceParentY + c.LayoutStyle.PositionType.PositionValue.y;

                            switch (c.LayoutStyle.Pivot.X.UnitType)
                            {
                                case UnitType.PERCENTAGE:
                                    {
                                        c.Rect.x -= c.Rect.width * c.LayoutStyle.Pivot.X.UnitValue * 0.01f;
                                        break;
                                    }
                                case UnitType.PIXEL:
                                    {
                                        c.Rect.x -= c.LayoutStyle.Pivot.X.UnitValue;
                                        break;
                                    }
                                case UnitType.KEYWORD:
                                    {
                                        break;
                                    }
                            }

                            switch (c.LayoutStyle.Pivot.Y.UnitType)
                            {
                                case UnitType.PERCENTAGE:
                                    {
                                        c.Rect.y -= c.Rect.height * c.LayoutStyle.Pivot.Y.UnitValue * 0.01f;
                                        break;
                                    }
                                case UnitType.PIXEL:
                                    {
                                        c.Rect.y -= c.LayoutStyle.Pivot.Y.UnitValue;
                                        break;
                                    }
                                case UnitType.KEYWORD:
                                    {
                                        break;
                                    }
                            }

                            FlowLayoutEntry.FlowPlacement(c, context);

                            break;
                        }
                    case PositionKeyword.SCREEN_SPACE:
                        {
                            c.Rect.x = c.LayoutStyle.PositionType.PositionValue.x;
                            c.Rect.y = c.LayoutStyle.PositionType.PositionValue.y;

                            FlowLayoutEntry.FlowPlacement(c, context);

                            break;
                        }
                    default:
                        {
                            throw new Exception("Position mode not implemented");
                        }
                }
            }
        }




    }
}
