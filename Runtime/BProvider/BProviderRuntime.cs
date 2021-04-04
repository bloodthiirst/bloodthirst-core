namespace Bloodthirst.Core.ServiceProvider
{
    public class BProviderRuntime
    {
        private static BProvider _instance;

        public static BProvider Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new BProvider();
                }

                return _instance;
            }
        }
    }
}