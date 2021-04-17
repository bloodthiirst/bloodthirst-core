using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FetchFromInjectedProvider : MonoBehaviour , IAwakePass
{
    [SerializeField]
    private ScriptableObject scriptableObject;

    void IAwakePass.Execute()
    {
        List<ScriptableObject> fetch = BProviderRuntime.Instance.GetInstances<ScriptableObject>().ToList();
        scriptableObject = fetch[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
