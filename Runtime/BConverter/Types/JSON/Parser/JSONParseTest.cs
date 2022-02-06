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
    private bool skipSpace;

    [SerializeField]
    private List<string> Tokens = new List<string>();

    [Button]
    private void TestParse()
    {
        JSONParser parser = new JSONParser() { SkipEmptySpace = skipSpace };
        TokenizerState<JSONTokenType> output = parser.TokenizeString(JsonInput);

        List<Token<JSONTokenType>> all = output.Tokens;

        Tokens.Clear();

        foreach(Token<JSONTokenType> t in all)
        {
            Tokens.Add(t.AsString());
        }
    }

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

    [Button]
    private void TestComplexParser()
    {
        TestStruct t = BConverterManger.FromJson<TestStruct>(JsonInput);
    }


}
