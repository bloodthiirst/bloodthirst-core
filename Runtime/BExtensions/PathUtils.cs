using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Scripts.Utils
{
    public static class PathUtils
    {
        public static float LineLength(this IList<Vector3> vecs)
        {
            if (vecs.Count < 2)
                return 0;

            float length = 0;

            Vector3 prev = vecs[0];

            for (int i = 1; i < vecs.Count; i++)
            {
                Vector3 curr = vecs[i];

                length += (curr - prev).magnitude;

                prev = curr;
            }

            return length;
        }

        public static float LineLength(this IList<Vector3> vecs , int start , int endIndex)
        {
            if (vecs.Count < 2)
                return 0;

            float length = 0;

            Vector3 prev = vecs[start];

            for (int i = start + 1; i < endIndex + 1; i++)
            {
                Vector3 curr = vecs[i];

                length += (curr - prev).magnitude;

                prev = curr;
            }

            return length;
        }


        public enum WALKING_RESULT
        {
            IDLE,
            WALKING,
            DONE
        }

        [Serializable]
        public struct WalkingState
        {
            public int pathIndex;

            public Vector3 currentPosition;

            public double distanceWalked;
        }

        public static WALKING_RESULT WalkAlongPath(Vector3[] path, float speed, ref WalkingState walkingState)
        {
            float distanceToWalk = speed;

            int index = walkingState.pathIndex;

            // we try walking down the path
            while (index < path.Length - 1)
            {
                Vector3 nextPos = path[index + 1];
                Vector3 segmentVec = nextPos - walkingState.currentPosition;
                float segmentLength = segmentVec.magnitude;

                // if we can walk more than the current segment
                // snap to the next point
                // and continue
                if (distanceToWalk >= segmentLength)
                {
                    walkingState.currentPosition = nextPos;
                    walkingState.distanceWalked += segmentLength;
                    distanceToWalk -= segmentLength;
                    index++;
                    continue;
                }

                // walk along the segment using the rest of the distance left
                walkingState.currentPosition += segmentVec.normalized * distanceToWalk;
                walkingState.distanceWalked += distanceToWalk;
                break;
            }

            walkingState.pathIndex = index;

            // if we reached the end of the path
            if (walkingState.pathIndex == path.Length - 1)
            {
                return WALKING_RESULT.DONE;
            }
            else
            {
                return WALKING_RESULT.WALKING;
            }
        }

        public static WALKING_RESULT WalkAlongPath(Vector3[] path, double speed, ref WalkingState walkingState, int finalIndex)
        {
            double distanceToWalk = speed;

            // we try walking down the path
            for (; walkingState.pathIndex < finalIndex; walkingState.pathIndex++)
            {
                Vector3 vecToEndOfSegment = path[walkingState.pathIndex + 1] - walkingState.currentPosition;

                double distanceToEndOfSegement = Math.Sqrt(vecToEndOfSegment.x * vecToEndOfSegment.x + vecToEndOfSegment.y * vecToEndOfSegment.y + vecToEndOfSegment.z * vecToEndOfSegment.z);

                // if we can walk more than the current segment
                // snap to the next point
                // and continue
                if (distanceToWalk >= distanceToEndOfSegement || distanceToEndOfSegement <= float.Epsilon)
                {
                    walkingState.currentPosition = path[walkingState.pathIndex + 1];
                    walkingState.distanceWalked += distanceToEndOfSegement;
                    distanceToWalk -= distanceToEndOfSegement;
                    continue;
                }

                // walk along the segment using the rest of the distance left
                double scaler = distanceToWalk / distanceToEndOfSegement;
                walkingState.currentPosition.x += (float) (vecToEndOfSegment.x * scaler);
                walkingState.currentPosition.y += (float) (vecToEndOfSegment.y * scaler);
                walkingState.currentPosition.z += (float) (vecToEndOfSegment.z * scaler);

                walkingState.distanceWalked += distanceToWalk;

                return WALKING_RESULT.WALKING;
            }

            walkingState.currentPosition = path[finalIndex];

            return WALKING_RESULT.DONE;
        }
    }
}
