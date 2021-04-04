using UnityEngine.Assertions;

namespace Bloodthirst.Core.ServiceProvider
{
    public class BProviderRuntime
    {
        private static BProvider instance;

        public static void OverrideProvider(BProvider provider)
        {
            Assert.IsNotNull(provider);

            instance = provider;
        }

        public static BProvider Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new BProvider();
                }

                return instance;
            }
        }


    }
}