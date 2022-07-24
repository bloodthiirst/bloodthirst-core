using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.Profiling;

public class InOutArray_Basic_Correctness
{
    [DebuggerDisplay("{ToString(),nq}")]
    private class SomeClass
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public SomeClass(int Id)
        {
            this.Id = Id;
        }
        public override string ToString()
        {
            return Id.ToString();
        }
    }

    [Test]
    public void Remove_Then_Add_Test()
    {
        InOutArray<SomeClass> inOutArray = new InOutArray<SomeClass>(5);

        SomeClass ins1 = new SomeClass(1);
        SomeClass ins2 = new SomeClass(2);
        SomeClass ins3 = new SomeClass(3);
        SomeClass ins4 = new SomeClass(4);

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

        SomeClass ins5 = new SomeClass(5);

        inOutArray.Add(ins5);

        bool found5 = inOutArray.Contains(ins5);

        inOutArray.Add(ins5);
    }

    [Test]
    public void Remove_Then_Add_Then_Iterate_Test()
    {
        InOutArray<SomeClass> inOutArray = new InOutArray<SomeClass>(5);

        SomeClass ins0 = new SomeClass(0);
        SomeClass ins1 = new SomeClass(1);
        SomeClass ins2 = new SomeClass(2);
        SomeClass ins3 = new SomeClass(3);

        inOutArray.Add(ins0);
        inOutArray.Add(ins1);
        inOutArray.Add(ins2);
        inOutArray.Add(ins3);

        UnityEngine.Debug.Log("Added 4 items");
        inOutArray.Iterate((item, index) =>
        {
            UnityEngine.Debug.Log($"id : {item.Id} -> index : {index}");
        });

        bool added = inOutArray.Contains(ins2);
        bool found = inOutArray.Contains(ins2);

        Assert.IsTrue(added);
        Assert.IsTrue(found);

        inOutArray.RemoveAt(0);

        bool deleted = !inOutArray.Contains(ins2);


        UnityEngine.Debug.Log("removed first item");
        inOutArray.Iterate((item, index) =>
        {
            UnityEngine.Debug.Log($"id : {item.Id} -> index : {index}");
        });


        SomeClass ins4 = new SomeClass(4);

        inOutArray.Add(ins4);

        UnityEngine.Debug.Log("Added 1 item with id 4");
        inOutArray.Iterate((item, index) =>
        {
            UnityEngine.Debug.Log($"id : {item.Id} -> index : {index}");
        });

        bool found4 = inOutArray.Contains(ins4);

        SomeClass ins5 = new SomeClass(5);

        inOutArray.Add(ins5);


        UnityEngine.Debug.Log("Added 1 item with id 5");
        inOutArray.Iterate((item, index) =>
        {
            UnityEngine.Debug.Log($"id : {item.Id} -> index : {index}");
        });

        inOutArray.RemoveAt(3);

        UnityEngine.Debug.Log("Removed 1 item with index 3");
        inOutArray.Iterate((item, index) =>
        {
            UnityEngine.Debug.Log($"id : {item.Id} -> index : {index}");
        });

    }



}
