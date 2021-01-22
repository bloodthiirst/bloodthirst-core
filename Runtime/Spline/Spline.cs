using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Utils
{
    public class Spline<T> where T : INodePosition
    {
        private List<Segment<T>> Segments { get; set; }

        private List<Vector3> PointsCache { get; set; }

        public float HandleLength { get; set; }

        public bool InvertHandlesLengths { get; set; }

        public bool NormalizeHandles { get; set; }

        public int SegmentCount => Segments.Count;

        public Segment<T> this[int index]
        {
            get
            {
                return Segments[index];
            }
            set
            {
                Segments[index] = value;
            }
        }

        public Spline()
        {
            Segments = new List<Segment<T>>();
            PointsCache = new List<Vector3>();
        }

        /// <summary>
        /// Handles passed should be in local space
        /// </summary>
        /// <param name = "nodePath" ></ param >
        /// < param name="StartHandle1"></param>
        /// <param name = "EndHandle2" ></ param >
        public void Initialize(List<T> nodePath, Vector3? StartHandle1 = null, Vector3? EndHandle2 = null, bool softCorners = false)
        {
            PointsCache.Clear();

            for (int i = 0; i < nodePath.Count; i++)
            {
                PointsCache.Add(nodePath[i].Position);
            }

            if (softCorners)
            {
                SoftEdgedPath(PointsCache, nodePath, StartHandle1, EndHandle2);
            }
            else
            {
                SharpCornersPath(PointsCache, nodePath, StartHandle1, EndHandle2);
            }
        }

        private void SharpCornersPath(IList<Vector3> Points, IList<T> nodePath, Vector3? StartHandle1 = null, Vector3? EndHandle2 = null)
        {
            //clear previous segment
            Segments.Clear();

            if (Points.Count == 2)
            {
                Segment<T> segment = new Segment<T>();

                segment.Point1 = Points[0];
                segment.Point2 = Points[1];
                segment.Node1 = nodePath[0];
                segment.Node2 = nodePath[1];

                segment.Handle1 = Vector3.Lerp(segment.Point1, segment.Point2, 0.33f);
                segment.Handle2 = Vector3.Lerp(segment.Point1, segment.Point2, 0.66f);

                Segments.Add(segment);

                FixFirstAndLastHandles(StartHandle1, EndHandle2);
                return;
            }

            Vector3? lastHalfway = null;


            int i = 0;
            //initialize the points

            while (i < Points.Count - 1)
            {
                var p1 = Points[i];
                var p2 = Points[i + 1];

                //check if enough points are left

                if (i == Points.Count - 2)
                {
                    Segment<T> segment = new Segment<T>();

                    if (lastHalfway.HasValue)
                    {
                        segment.Point1 = lastHalfway.Value;
                        lastHalfway = null;
                    }
                    else
                    {
                        segment.Point1 = p1;
                    }

                    segment.Point2 = p2;
                    segment.Node1 = nodePath[i];
                    segment.Node2 = nodePath[i + 1];

                    segment.Handle1 = Vector3.Lerp(segment.Point1, segment.Point2, 0.33f);
                    segment.Handle2 = Vector3.Lerp(segment.Point1, segment.Point2, 0.66f);


                    Segments.Add(segment);
                    break;
                }


                var p3 = Points[i + 2];

                var firstVec = p2 - p1;
                var secVec = p3 - p2;

                firstVec.Normalize();
                secVec.Normalize();

                var up = Vector3.Cross(firstVec, secVec).normalized;

                float angle = Vector3.SignedAngle(firstVec, secVec, up);

                angle = Mathf.Abs(angle);

                //if not curve
                if (angle < 60)
                {
                    Segment<T> segment = new Segment<T>();

                    if (lastHalfway.HasValue)
                    {
                        segment.Point1 = lastHalfway.Value;
                        lastHalfway = null;
                    }
                    else
                    {
                        segment.Point1 = p1;
                    }


                    segment.Point2 = p2;
                    segment.Node1 = nodePath[i];
                    segment.Node2 = nodePath[i + 1];

                    segment.Handle1 = Vector3.Lerp(segment.Point1, segment.Point2, 0.33f);
                    segment.Handle2 = Vector3.Lerp(segment.Point1, segment.Point2, 0.66f);

                    Segments.Add(segment);

                    i++;
                    continue;
                }

                //if curve
                else
                {
                    Segment<T> segment = new Segment<T>();

                    //if halfway exist
                    //then just link it to the second half way
                    if (!lastHalfway.HasValue)
                    {
                        Segment<T> additionalStartSeg = new Segment<T>();
                        lastHalfway = Vector3.Lerp(p1, p2, 0.5f);
                        additionalStartSeg.Point1 = p1;
                        additionalStartSeg.Point2 = lastHalfway.Value;

                        additionalStartSeg.Handle1 = Vector3.Lerp(additionalStartSeg.Point1, additionalStartSeg.Point2, 0.33f);
                        additionalStartSeg.Handle2 = Vector3.Lerp(additionalStartSeg.Point1, additionalStartSeg.Point2, 0.66f);

                        Segments.Add(additionalStartSeg);
                    }


                    segment.Point1 = lastHalfway.Value;

                    lastHalfway = Vector3.Lerp(p2, p3, 0.5f);

                    segment.Point2 = lastHalfway.Value;

                    segment.Node1 = nodePath[i];
                    segment.Node2 = nodePath[i + 2];

                    segment.Handle1 = p2;
                    segment.Handle2 = p2;

                    Segments.Add(segment);



                    i++;
                    continue;
                }
            }

            //fix first and last handle
            FixFirstAndLastHandles(StartHandle1, EndHandle2);
        }

        public float GetTotalLength()
        {
            float l = 0;

            for (int i = 0; i < Segments.Count; i++)
            {
                l += Segments[i].Length;
            }

            return l;
        }

        private void SoftEdgedPath(IList<Vector3> Points, IList<T> nodePath, Vector3? StartHandle1 = null, Vector3? EndHandle2 = null)
        {
            // clear previous segment
            Segments.Clear();

            if (Points.Count == 2)
            {
                Segment<T> segment = new Segment<T>();

                segment.Point1 = Points[0];
                segment.Point2 = Points[1];
                segment.Node1 = nodePath[0];
                segment.Node2 = nodePath[1];

                segment.Handle1 = Vector3.Lerp(segment.Point1, segment.Point2, 0.33f);
                segment.Handle2 = Vector3.Lerp(segment.Point1, segment.Point2, 0.66f);

                Segments.Add(segment);

                FixFirstAndLastHandles(StartHandle1, EndHandle2);
                return;
            }

            Segment<T> lastSegment = null;


            // initialize the points
            for (int i = 0; i < Points.Count - 1; i++)
            {
                Segment<T> segment = new Segment<T>();

                segment.Point1 = Points[i];
                segment.Point2 = Points[i + 1];
                segment.Node1 = nodePath[i];
                segment.Node2 = nodePath[i + 1];

                if (segment.Node1.Equals(segment.Node2))
                {
                    continue;
                }

                // if first seg
                if (lastSegment == null)
                {
                    Segments.Add(segment);
                    lastSegment = segment;
                    continue;
                }

                if (segment.Point2 == lastSegment.Point2 && segment.Point1 == lastSegment.Point2)
                {
                    continue;
                }

                if (segment.Point1 == segment.Point2)
                {
                    continue;
                }

                Segments.Add(segment);
                lastSegment = segment;
            }

            // fix alignment
            FixHandleAlignement();


            // fix first and last handle
            FixFirstAndLastHandles(StartHandle1, EndHandle2);
        }

        private void FixFirstAndLastHandles(Vector3? StartHandle1, Vector3? EndHandle2)
        {
            // put the first and last handles
            // first
            if (StartHandle1.HasValue)
            {
                Segment<T> segment = Segments[0];
                segment.Handle1 = segment.Point1 + ((segment.Point1 - StartHandle1.Value).normalized * HandleLength);
            }
            else
            {
                Segment<T> seg = Segments[0];
                seg.Handle1 = Vector3.Lerp(seg.Point1, seg.Handle2, 0.5f);
            }

            // last
            if (EndHandle2.HasValue)
            {
                Segment<T> segment = Segments[SegmentCount - 1];
                segment.Handle2 = segment.Point2 + ((segment.Point2 - EndHandle2.Value).normalized * HandleLength);
            }
            else
            {
                Segment<T> segment = Segments[SegmentCount - 1];
                segment.Handle2 = Vector3.Lerp(segment.Point2, segment.Handle1, 0.5f);
            }
        }

        private void FixHandleAlignement()
        {
            for (int i = 0; i < SegmentCount - 1; i++)
            {
                Segment<T> seg = Segments[i];

                Segment<T> next = Segments[i + 1];

                FixHandles(seg, next);
            }
        }

        private void FixHandles(Segment<T> seg, Segment<T> next)
        {
            Vector3 middle = Vector3.Lerp(seg.Point1, next.Point2, 0.5f);

            // direction

            Vector3 middleToCenter = (middle - seg.Point2).normalized;

            Vector3 l = (seg.Point2 - seg.Point1);

            Vector3 r = (next.Point2 - next.Point1);

            float lengthCurrent = l.magnitude;

            float lengthNext = r.magnitude;

            l.Normalize();

            r.Normalize();

            Vector3 up = Vector3.Cross(l, r);

            Vector3 perpendicular = Vector3.Cross(middleToCenter, up);

            if (perpendicular == Vector3.zero)
            {

                seg.Handle2 = Vector3.Lerp(seg.Point2, seg.Point1, 0.33f);
                next.Handle1 = Vector3.Lerp(next.Point1, next.Point2, 0.33f);
                return;
            }

            Vector3 handlerCurrent = -perpendicular.normalized;
            Vector3 handlerNext = perpendicular.normalized;


            if (InvertHandlesLengths)
            {
                float tmp = lengthCurrent;
                lengthCurrent = lengthNext;
                lengthNext = tmp;
            }
            if (NormalizeHandles)
            {
                handlerCurrent *= lengthCurrent;
                handlerNext *= lengthNext;
            }
            else
            {
                handlerCurrent *= HandleLength;
                handlerNext *= HandleLength;
            }

            seg.Handle2 = seg.Point2 + handlerCurrent;
            next.Handle1 = seg.Point2 + handlerNext;
        }
    }
}
