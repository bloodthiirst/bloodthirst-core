using System;

namespace Bloodthirst.Socket.Core
{
    public class SocketIdentifier<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        private static SocketIdentifier<TIdentifier> instance;

        private static SocketIdentifier<TIdentifier> Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new SocketIdentifier<TIdentifier>();
                }

                return instance;
            }
        }

        /// <summary>
        /// Return the default value used to represent identity id
        /// </summary>
        /// <returns></returns>
        public static TIdentifier Get {
      
            get
            {
                return Instance.DefaultIdentifier();
            }
        }

        /// <summary>
        /// generate the identity id
        /// </summary>
        /// <returns></returns>
        protected virtual TIdentifier DefaultIdentifier()
        {
            return default;
        }
    }
}
