﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Scripts.Utils
{
    public static class PathUtils
    {
        public static void WalkDistance(IReadOnlyList<Vector3> pts, float distance, out Vector3 pos, out Vector3 dir)
        {
            Assert.IsTrue(pts.Count >= 2);
            Assert.IsTrue(distance >= 0);

            float walkedDist = distance;

            for(int i = 0; i < pts.Count - 1; ++i)
            {
                Vector3 curr = pts[i];
                Vector3 next = pts[i + 1];

                Vector3 currDir = next - curr;
                float dist = currDir.magnitude;
                
                if(dist < walkedDist)
                {
                    walkedDist -= dist;
                    continue;
                }

                pos = curr + (currDir / dist) * walkedDist;
                dir = currDir / dist;
                return;
            }

            pos = pts[pts.Count - 1];
            dir = (pos - pts[pts.Count - 2]).normalized;

        }
        public static float LineLength(this IList<Vector3> vecs)
        {
            return LineLength(vecs , 0, vecs.Count - 1);
        }

        public static float LineLength(this IList<Vector3> vecs, int start, int endIndex)
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

            public Vector3 lastWalkingDirection;

            public double totalDistanceWalked;
        }


        public static void NormalToManhattenPathConverter(IReadOnlyList<Vector3> normalPath , List<Vector3> manhatternPath) 
        {
            manhatternPath.Add(normalPath[0]);

            for(int i = 1; i < normalPath.Count; i++)
            {
                Vector3 curr = normalPath[i];
                Vector3 prev = normalPath[i - 1];

                Vector3 diff = curr - prev;

                if (diff.x == 0 || diff.z == 0)
                {
                    manhatternPath.Add(curr);
                    continue;
                }

                Vector3 longSide = default;
                Vector3 shortSide = default;

                float longDistance = default;
                float shortDistance = default;

                // select the long and short directions and distances
                {
                    if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
                    {
                        longSide = Vector3.right * Mathf.Sign(diff.x);
                        shortSide = Vector3.forward * Mathf.Sign(diff.z);
                        longDistance = Mathf.Abs(diff.x);
                        shortDistance = Mathf.Abs(diff.z);
                    }
                    else
                    {
                        longSide = Vector3.forward * Mathf.Sign(diff.z);
                        shortSide = Vector3.right * Mathf.Sign(diff.x);
                        longDistance = Mathf.Abs(diff.z);
                        shortDistance = Mathf.Abs(diff.x);
                    }
                }

                float totalStraightWalkDist = longDistance - shortDistance;
                Vector3 middlePoint = prev + (longSide * totalStraightWalkDist);

                manhatternPath.Add(middlePoint);
                manhatternPath.Add(curr);
            }
        }

        public static bool GetManhattenDisplacement(IReadOnlyList<Vector3> path, float speed, ref WalkingState walkingState, out Vector3 newPos, out Vector3 normal, out float distance)
        {
            if (walkingState.pathIndex == 0)
            {
                return GetNormalDisplacement(path, speed, ref walkingState, out newPos, out normal, out distance);
            }

            Vector3 start = path[walkingState.pathIndex - 1];
            Vector3 end = path[walkingState.pathIndex];

            Vector3 from = walkingState.currentPosition;
            Vector3 walkDiff = end - start;

            if (walkDiff.x == 0 || walkDiff.z == 0)
            {
                return GetNormalDisplacement(path, speed, ref walkingState, out newPos, out normal, out distance);
            }

            Vector3 longSide = default;
            Vector3 shortSide = default;

            float longDistance = default;
            float shortDistance = default;

            // select the long and short directions and distances
            {
                if (Mathf.Abs(walkDiff.x) > Mathf.Abs(walkDiff.z))
                {
                    longSide = Vector3.right * Mathf.Sign(walkDiff.x);
                    shortSide = Vector3.forward * Mathf.Sign(walkDiff.z);
                    longDistance = Mathf.Abs(walkDiff.x);
                    shortDistance = Mathf.Abs(walkDiff.z);
                }
                else
                {
                    longSide = Vector3.forward * Mathf.Sign(walkDiff.z);
                    shortSide = Vector3.right * Mathf.Sign(walkDiff.x);
                    longDistance = Mathf.Abs(walkDiff.z);
                    shortDistance = Mathf.Abs(walkDiff.x);
                }
            }

            float totalStraightWalkDist = longDistance - shortDistance;
            Vector3 straightWalkEnd = start + (longSide * totalStraightWalkDist);

            Vector3 fromDiff = from - start;
            float distFromStart = Vector3.Dot(fromDiff, longSide);
            float shorWalkDiff = totalStraightWalkDist - distFromStart;

            // if still doing the short distance
            if (distFromStart < totalStraightWalkDist)
            {
                float moveDist = Mathf.Min(speed, shorWalkDiff);
                newPos = from + (longSide * moveDist);
                normal = longSide;
                distance = moveDist;

                return false;
            }

            // beyond short dist
            {
                Vector3 totalDiagWalk = (end - straightWalkEnd);
                Vector3 currDiagWalk = (end - from);

                float totalDiagDist = totalDiagWalk.magnitude;
                float currDiagDist = currDiagWalk.magnitude;

                var dir = totalDiagWalk / totalDiagDist;

                float moveDist = Mathf.Min(speed, currDiagDist);

                newPos = from + (dir * moveDist);
                normal = dir;
                distance = moveDist;

                if (newPos == end)
                {
                    walkingState.pathIndex++;
                    return true;
                }

                return false;
            }
        }

        public static bool GetNormalDisplacement(IReadOnlyList<Vector3> path, float speed, ref WalkingState walkingState, out Vector3 newPos, out Vector3 normal, out float distance)
        {
            Vector3 from = walkingState.currentPosition;
            Vector3 to = path[walkingState.pathIndex];

            Vector3 disp = to - from;
            float diff = disp.magnitude;
            Vector3 dir = disp / diff;

            normal = dir;

            // if we have enough speed to reach the next point
            // then just snap to it directly
            if (diff <= speed)
            {
                distance = diff;
                newPos = to;
                walkingState.pathIndex++;
                return true;
            }

            distance = speed;
            newPos = from + (speed * dir);

            return false;
        }

        [Obsolete("Instead of using this every frame , consider converting the path a manhatten path using the \"NormalToManhattenPathConverter\" method")]
        public static WALKING_RESULT WalkAlongPathManhatten(IReadOnlyList<Vector3> path, float speed, ref WalkingState walkingState)
        {
            float distanceToWalk = speed;

            // we try walking down the path
            while (walkingState.pathIndex < path.Count)
            {
                bool reachedPoint = GetManhattenDisplacement(path, distanceToWalk, ref walkingState, out Vector3 pos, out Vector3 normal, out float walked);

                walkingState.currentPosition = pos;
                walkingState.totalDistanceWalked += walked;
                walkingState.lastWalkingDirection = normal;
                distanceToWalk -= walked;

                if (!reachedPoint)
                    break;
            }

            // if we reached the end of the path
            if (walkingState.pathIndex == path.Count)
            {
                return WALKING_RESULT.DONE;
            }

            return WALKING_RESULT.WALKING;
        }

        public static WALKING_RESULT WalkAlongPath(IReadOnlyList<Vector3> path, float speed, ref WalkingState walkingState)
        {
            float distanceToWalk = speed;

            // we try walking down the path
            while (walkingState.pathIndex < path.Count)
            {
                bool reachedPoint = GetNormalDisplacement(path, distanceToWalk, ref walkingState, out Vector3 pos, out Vector3 normal, out float walked);

                walkingState.currentPosition = pos;
                walkingState.totalDistanceWalked += walked;
                walkingState.lastWalkingDirection = normal;
                distanceToWalk -= walked;

                if (!reachedPoint)
                    break;
            }

            // if we reached the end of the path
            if (walkingState.pathIndex == path.Count)
            {
                walkingState.currentPosition = path[path.Count - 1];
                return WALKING_RESULT.DONE;
            }

            return WALKING_RESULT.WALKING;
        }
    }
}
