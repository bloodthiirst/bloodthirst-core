using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    public class FlowBlock : IFlow
    {
        public void FlowHeight(ILayoutBox layoutBox, FlowContext context)
        {
            // height section
            List<ILayoutBox> autoHeightBoxes = new List<ILayoutBox>();

            //recursing list
            List<ILayoutBox> recursiveLayouts = new List<ILayoutBox>();

            float accumulatedHeight = 0;

            foreach (ILayoutBox childLayout in layoutBox.ChildLayouts)
            {
                bool preFlowHeight = childLayout.LayoutStyle.Height.KeywordValue == UnitKeyword.Content && childLayout.LayoutStyle.Height.UnitType == UnitType.KEYWORD;

                if (!preFlowHeight)
                {
                    recursiveLayouts.Add(childLayout);
                    continue;
                }

                FlowLayoutEntry.FlowHeight(childLayout, context);

                accumulatedHeight += childLayout.Rect.height;
            }


            for (int i = 0; i < layoutBox.ChildLayouts.Count; i++)
            {
                ILayoutBox childLayout = layoutBox.ChildLayouts[i];

                float h = 0;

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
                                h = childLayout.Rect.height;
                                break;
                            }

                            break;
                        }
                    case UnitType.PERCENTAGE:
                        {
                            h = childLayout.ParentLayout.Rect.height * (childLayout.LayoutStyle.Height.UnitValue / 100f);
                            childLayout.Rect.height = h;
                            break;
                        }
                    case UnitType.PIXEL:
                        {
                            h = childLayout.LayoutStyle.Height.UnitValue;
                            childLayout.Rect.height = h;
                            break;
                        }
                }

                switch (childLayout.LayoutStyle.PositionType.PositionKeyword)
                {
                    case PositionKeyword.DISPLAY_MODE:
                        {
                            accumulatedHeight += h;
                            break;
                        }
                    default:
                        break;
                }
            }

            // the rest of the width to divide between the auto boxes
            float heightLeft = Mathf.Abs(layoutBox.Rect.height - accumulatedHeight);
            float hPerAuto = heightLeft / autoHeightBoxes.Count;
            foreach (ILayoutBox c in autoHeightBoxes)
            {
                c.Rect.height = hPerAuto;
            }

            // stretch to content
            if (layoutBox.LayoutStyle.Height.UnitType == UnitType.KEYWORD && layoutBox.LayoutStyle.Height.KeywordValue == UnitKeyword.Content)
            {
                layoutBox.Rect.height = layoutBox.GetChildrenHeightSum();
            }

            foreach (ILayoutBox c in recursiveLayouts)
            {
                FlowLayoutEntry.FlowHeight(c, context);
            }
        }
        public void FlowWidth(ILayoutBox layoutBox, FlowContext context)
        {
            // width section
            List<ILayoutBox> autoWidthtBoxes = new List<ILayoutBox>();
            float maxWidth = layoutBox.Rect.width;

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
                                autoWidthtBoxes.Add(childLayout);
                                break;
                            }

                            if (childLayout.LayoutStyle.Height.KeywordValue == UnitKeyword.BasedOnHeight)
                            {
                                FlowLayoutEntry.FlowHeight(childLayout, context);
                                childLayout.Rect.width = childLayout.Rect.height * childLayout.LayoutStyle.Width.UnitValue * 0.01f;
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
                            float w = childLayout.LayoutStyle.Width.UnitValue;
                            maxWidth = Mathf.Max(maxWidth, w);
                            childLayout.Rect.width = w;
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
                switch (childLayout.LayoutStyle.Width.UnitType)
                {
                    case UnitType.PERCENTAGE:
                        {
                            float w = childLayout.ParentLayout.Rect.width * (maxWidth / 100f);
                            maxWidth = Mathf.Max(maxWidth, w);
                            childLayout.Rect.width = w;
                            break;
                        }
                }
            }

            // the rest of the width to divide between the auto boxes
            foreach (ILayoutBox c in autoWidthtBoxes)
            {
                c.Rect.width = layoutBox.Rect.width;
            }

            // stretch to content
            if (layoutBox.LayoutStyle.Width.UnitType == UnitType.KEYWORD && layoutBox.LayoutStyle.Width.KeywordValue == UnitKeyword.Content)
            {
                layoutBox.Rect.width = layoutBox.GetChildrenWidthSum();
            }

            foreach (ILayoutBox c in layoutBox.ChildLayouts)
            {
                FlowLayoutEntry.FlowWidth(c, context);
            }
        }

        public void FlowPlacement(ILayoutBox layoutBox, FlowContext context)
        {
            float childOffsetY = 0;

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
                            c.Rect.y = worldSpaceParentY + childOffsetY;
                            c.Rect.x = worldSpaceParentX;

                            FlowLayoutEntry.FlowPlacement(c, context);

                            childOffsetY += c.Rect.height;

                            break;
                        }
                    case PositionKeyword.PARENT_SPACE:
                        {
                            c.Rect.x = worldSpaceParentX + c.LayoutStyle.PositionType.PositionValue.x;
                            c.Rect.y = worldSpaceParentY + c.LayoutStyle.PositionType.PositionValue.y;

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
