using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    public class FlowBlock : IFlow
    {
        public void Flow(ILayoutBox layoutBox, FlowContext context)
        {
            List<ILayoutBox> contentSubLayouts = FlowContentLayouts(layoutBox, context).ToList();

            FlowHeight(layoutBox, context);

            FlowWidth(layoutBox, context);

            FlowPlacement(layoutBox, context);
        }

        private IEnumerable<ILayoutBox> FlowContentLayouts(ILayoutBox layoutBox, FlowContext context)
        {
            foreach (ILayoutBox childLayout in layoutBox.ChildLayouts)
            {
                bool preFlowWidth = childLayout.LayoutStyle.Width.KeywordValue == UnitKeyword.Content && childLayout.LayoutStyle.Width.UnitType == UnitType.KEYWORD;
                bool preFlowHeight = childLayout.LayoutStyle.Height.KeywordValue == UnitKeyword.Content && childLayout.LayoutStyle.Height.UnitType == UnitType.KEYWORD;

                bool flowChildren = preFlowHeight || preFlowWidth;

                if (!flowChildren)
                    continue;

                FlowLayoutEntry.Flow(childLayout, context);

                yield return childLayout;
            }
        }


        public void FlowHeight(ILayoutBox layoutBox, FlowContext context)
        {
            // width section
            List<ILayoutBox> autoHeightBoxes = new List<ILayoutBox>();
            float accumulatedHeight = 0;

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
                            float h = childLayout.ParentLayout.Rect.height * (childLayout.LayoutStyle.Height.UnitValue / 100f);
                            accumulatedHeight += h;
                            childLayout.Rect.height = h;
                            break;
                        }
                    case UnitType.PIXEL:
                        {
                            float h = childLayout.LayoutStyle.Height.UnitValue;
                            accumulatedHeight += h;
                            childLayout.Rect.height = h;
                            break;
                        }
                }
            }

            // if we have boxes with auto width
            if (autoHeightBoxes.Count != 0)
            {
                // the rest of the width to divide between the auto boxes
                float heightLeft = Mathf.Abs(layoutBox.Rect.height - accumulatedHeight);
                float hPerAuto = heightLeft / autoHeightBoxes.Count;
                foreach (ILayoutBox c in autoHeightBoxes)
                {
                    c.Rect.height = hPerAuto;
                }
            }

            // stretch to content
            if (layoutBox.LayoutStyle.Height.UnitType == UnitType.KEYWORD && layoutBox.LayoutStyle.Height.KeywordValue == UnitKeyword.Content)
            {
                layoutBox.Rect.height = layoutBox.GetChildrenHeightSum();
                return;
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
                return;
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
                c.Rect.y = worldSpaceParentY + childOffsetY;
                c.Rect.x = worldSpaceParentX;

                if (!context.LayoutsWithFlowApplied.Contains(c))
                {
                    FlowLayoutEntry.Flow(c, context);
                    c.PostFlow();
                }

                childOffsetY += c.Rect.height;
            }
        }




    }
}
