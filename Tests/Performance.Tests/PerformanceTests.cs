using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PerformanceTests
{
    // A Test behaves as an ordinary method
    [Test]
    [TestCase(10_000_000)]
    public void FloatOperations(int iterations)
    {
        Stopwatch sw = new Stopwatch();

        long multiplication = 0;
        long division = 0;
        long addition = 0;
        long subtraction = 0;

        for (int i = 0; i < iterations; ++i)
        {
            float a = Random.Range(0.1f, 1f);
            float b = Random.Range(0.1f, 1f);

            float c = default;

            // multiplication
            {
                sw.Restart();
                c = a * b;
                multiplication += sw.ElapsedTicks;
            }

            // division
            {
                sw.Restart();
                c = a / b;
                division += sw.ElapsedTicks;
            }

            // addition
            {
                sw.Restart();
                c = a + b;
                addition += sw.ElapsedTicks;
            }

            // subtraction
            {
                sw.Restart();
                c = a - b;
                subtraction += sw.ElapsedTicks;
            }
        }

        UnityEngine.Debug.Log(
            $"For {iterations} the results (in Ticks) are\n" +
            $"- Multiplication : \t{multiplication}\n" +
            $"- Division : \t{division}\n" +
            $"- Addition : \t{addition}\n" +
            $"- Subtraction : \t{subtraction}\n");
    }
    [Test]
    [TestCase(10_000_000)]
    public void IntOperations(int iterations)
    {
        Stopwatch sw = new Stopwatch();

        long multiplication = 0;
        long division = 0;
        long addition = 0;
        long subtraction = 0;

        for (int i = 0; i < iterations; ++i)
        {
            int a = Random.Range(1, 2);
            int b = Random.Range(1, 2);

            int c = default;

            // multiplication
            {
                sw.Restart();
                c = a * b;
                multiplication += sw.ElapsedTicks;
            }

            // division
            {
                sw.Restart();
                c = a / b;
                division += sw.ElapsedTicks;
            }

            // addition
            {
                sw.Restart();
                c = a + b;
                addition += sw.ElapsedTicks;
            }

            // subtraction
            {
                sw.Restart();
                c = a - b;
                subtraction += sw.ElapsedTicks;
            }
        }

        UnityEngine.Debug.Log(
            $"For {iterations} the results (in Ticks) are\n" +
            $"- Multiplication : \t{multiplication}\n" +
            $"- Division : \t{division}\n" +
            $"- Addition : \t{addition}\n" +
            $"- Subtraction : \t{subtraction}\n");
    }

    [Test]
    [TestCase(10_000_000)]
    public void IntOperationsBatched(int iterations)
    {
        Stopwatch sw = new Stopwatch();

        long multiplication = 0;
        int it = 0;
        int a = Random.Range(1, 2);
        int b = Random.Range(2, 3);
        sw.Restart();
        while (it < iterations)
        {
            it++;
        }

        multiplication = sw.ElapsedTicks;

        UnityEngine.Debug.Log(
            $"For {iterations} the results (in Ticks) are\n" +
            $"- Multiplication : \t{multiplication}");
    }
}
