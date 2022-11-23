using Bloodthirst.Core.BProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Scripts.Core.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(MouseUtils))]
    [RequireComponent(typeof(MouseUtils))]
    public class MouseUtilsAdapter : MonoBehaviour, ISetupSingletonPass
    {
        void ISetupSingletonPass.Execute()
        {
            MouseUtils mouseUtils = GetComponent<MouseUtils>();

            Assert.IsNotNull(mouseUtils);

            BProviderRuntime.Instance.RegisterSingleton<MouseUtils, MouseUtils>(mouseUtils);
        }
    }
}
