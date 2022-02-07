using Bloodthirst.BDeepCopy;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONParseTest : MonoBehaviour
{
    [SerializeField]
    [TextArea(10, 50)]
    private string JsonInput;

    [SerializeField]
    [TextArea(10, 50)]
    private string JsonOutput;

    private class TestStruct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        private Dictionary<int , Info> Contacts;
    }

    private class Info
    {
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
    }

    public class HasUnityRef
    {
        public int Id { get; set; }

        public GameObject go { get; set; }
    }


    [Button]
    private void TestComplexParser()
    {
        TestStruct original = BConverterManger.FromJson<TestStruct>(JsonInput);
        JsonOutput = BConverterManger.ToJson(original);
        TestStruct copy = BConverterManger.FromJson<TestStruct>(JsonOutput);
    }

    [Title("Unity ref test")]
    [SerializeField]
    private List<UnityEngine.Object> unityObjects;

    [Button]
    private void TestUnityObject()
    {
        HasUnityRef original = new HasUnityRef() {  Id = 59 ,  go = gameObject };

        BConverterSettings ctx = new BConverterSettings()
        {
            CustomContext = new UnityObjectContext() { UnityObjects = unityObjects }
        };

        JsonOutput = BConverterManger.ToJson(original , ctx);
    }


}
