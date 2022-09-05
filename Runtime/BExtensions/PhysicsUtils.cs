using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Scripts.Utils
{
    public static class PhysicsUtils
    {
        public static double SolveForDistanceQuad(double vMax, double time)
        {
            return (vMax * (time * time * time)) / 3;
        }

        public static double SolveForDurationQuad(double vMax, double distance)
        {
            double tCube = (3 * distance) / vMax;
            return Math.Pow(tCube, 1 / 3d);
        }

        /// <summary>
        /// Gives duration needed with const acceleration based on : duration , t0 speed and distance
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="initialSpeed"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static float SolveForSpeed(float initSpeed, float acceleration, float displacement)
        {
            // v * v = (u * u) + ( 2 * acceleration * displacement)
            float v = MathF.Sqrt((initSpeed * initSpeed) + (2 * acceleration * displacement));
            return v;
        }

        /// <summary>
        /// Gives duration needed with const acceleration based on : duration , t0 speed and distance
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="initialSpeed"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static float SolveForDuration(float initSpeed, float speed, float displacement)
        {
            // s = 0.5 ( u + v ) t
            // t = s / (0.5 ( u + v ))
            float t = displacement / (0.5f * (initSpeed + speed));
            return t;
        }


        /// <summary>
        /// Gives the acceleration needed based on : duration , t0 speed and distance
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="initialSpeed"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static float SolveAcceleration(float duration, float initialSpeed, float distance)
        {
            // inital speed
            float u = initialSpeed;

            // final displacement
            float s = distance;

            // time taken to reach he final displacement
            float t = duration;

            float a = (s - (u * t)) / (0.5f * t * t);

            return a;
        }

        /// <summary>
        /// Gives the acceleration needed based on : duration , t0 speed and distance
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="initialSpeed"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static float GetAcceleration(float speed, float initialSpeed, float distance)
        {
            // inital speed
            float u = initialSpeed;

            // final displacement
            float s = distance;

            // time taken to reach he final displacement
            float v = speed;

            float a = ((v * v) - (u * u)) / (2 * s);

            return a;
        }
    }
}
