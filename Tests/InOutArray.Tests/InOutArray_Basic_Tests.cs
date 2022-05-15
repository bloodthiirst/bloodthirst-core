using NUnit.Framework;
using System.Collections.Generic;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.Profiling;

public class InOutArray_Basic_Tests
{
    private class SomeClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Test]
    public void Add_Test()
    {
        InOutArray<SomeClass> inOutArray = new InOutArray<SomeClass>(20);

        SomeClass ins = new SomeClass();
        inOutArray.Add(ins);

        bool found = inOutArray.Contains(ins);

        if (!found)
        {
            Assert.Fail("Didn't find object");
        }
        else
        {
            Assert.Pass("Found element");
        }

    }


    [Test]
    public void Remove_Test()
    {
        InOutArray<SomeClass> inOutArray = new InOutArray<SomeClass>(20);

        SomeClass ins1 = new SomeClass();
        SomeClass ins2 = new SomeClass();
        SomeClass ins3 = new SomeClass();
        SomeClass ins4 = new SomeClass();
        inOutArray.Add(ins1);
        inOutArray.Add(ins2);
        inOutArray.Add(ins3);
        inOutArray.Add(ins4);

        bool added = inOutArray.Contains(ins3);
        bool found = inOutArray.Contains(ins3);

        Assert.IsTrue(added);
        Assert.IsTrue(found);

        inOutArray.RemoveAt(0);

        bool deleted = !inOutArray.Contains(ins3);

        if (!deleted)
        {
            Assert.Fail("Didn't deleted object");
        }
        else
        {
            Assert.Pass("deleted element");
        }
    }


    [Test]
    public void Iterate_Test()
    {
        InOutArray<SomeClass> inOutArray = new InOutArray<SomeClass>(20);

        SomeClass ins = new SomeClass();
        inOutArray.Add(ins);
        inOutArray.Add(ins);
        inOutArray.Add(ins);
        inOutArray.Add(ins);

        inOutArray.Iterate((item, index) =>
        {
            Debug.Log(index);
        });

    }

    [Test]
    public void Add_RemoveAt_Iterate_Gap_Test()
    {
        InOutArray<SomeClass> inOutArray = new InOutArray<SomeClass>(20);

        SomeClass ins = new SomeClass();
        inOutArray.Add(ins);
        inOutArray.Add(ins);
        inOutArray.Add(ins);
        inOutArray.Add(ins);


        inOutArray.Iterate((item, index) =>
        {
            Debug.Log(index);
        });

        inOutArray.RemoveAt(2);

        inOutArray.Iterate((item, index) =>
        {
            Debug.Log(index);
        });

        inOutArray.Add(ins);


        inOutArray.Iterate((item, index) =>
        {
            Debug.Log(index);
        });
    }

    [Test]
    [Performance]
    public void Performance_Vs_List_Adding_Test()
    {
        int count = 2_000_00;
        int passes = 1;



        InOutArray<SomeClass> inOutArray = new InOutArray<SomeClass>(count);
        List<SomeClass> list = new List<SomeClass>();

        SomeClass ins = new SomeClass();

        Measure.Method(() =>
        {
            for (int i = 0; i < count; i++)
            {
                inOutArray.Add(ins);
            }
        })
        .SetUp(() =>
        {
            inOutArray.Clear();
        })
        .SampleGroup(new SampleGroup($"Adding {count} elements => InOutArray", SampleUnit.Millisecond))
        .IterationsPerMeasurement(1)
        .MeasurementCount(10)
        .GC()
        .Run();

        Measure.Method(() =>
        {
            for (int i = 0; i < count; i++)
            {
                list.Add(ins);
            }
        })
       .SetUp(() =>
        {
            list.Clear();
        })
        .SampleGroup(new SampleGroup($"Adding {count} elements => List", SampleUnit.Millisecond))
        .IterationsPerMeasurement(1)
        .MeasurementCount(10)
        .GC()
        .Run();

    }

    [Test]
    [Performance]
    public void Performance_Vs_List_Removing_Test()
    {
        int count = 2_000_00;
        int passes = 1;

        InOutArray<SomeClass> inOutArray = new InOutArray<SomeClass>(count);
        List<SomeClass> list = new List<SomeClass>();

        SomeClass ins = new SomeClass();

        Measure
        .Method(() =>
        {
            for (int i = 0; i < count; i++)
            {
                inOutArray.RemoveAt(i);
            }

        })
        .SetUp(() =>
        {
            inOutArray.Clear();
            for (int i = 0; i < count; i++)
            {
                inOutArray.Add(ins);
            }
        })
        .SampleGroup(new SampleGroup($"Removing {count} elements => InOutArray", SampleUnit.Millisecond))
        .WarmupCount(5)
        .MeasurementCount(10)
        .GC()
        .Run();


        Measure
        .Method(() =>
        {

            for (int i = 0; i < count; i++)
            {
                list.RemoveAt(0);
            }

        })
        .SetUp(() =>
        {
            inOutArray.Clear();
            for (int i = 0; i < count; i++)
            {
                list.Add(ins);
            }
        })
        .SampleGroup(new SampleGroup($"Removing {count} elements => List", SampleUnit.Millisecond))
        .WarmupCount(5)
        .MeasurementCount(10)
        .GC()
        .Run();

    }
}
