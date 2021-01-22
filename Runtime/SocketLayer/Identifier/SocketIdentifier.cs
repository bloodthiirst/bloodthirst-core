using System;

namespace Bloodthirst.Socket.Core
{
    public abstract class SocketIdentifier<TType, TIdentifier>
        where TType : SocketIdentifier<TType, TIdentifier>, new()
        where TIdentifier : IComparable<TIdentifier>
    {
        private static TType instance;

        private static TType Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TType();
                }

                return instance;
            }
        }

        /// <summary>
        /// Return the default value used to represent cient identity id
        /// </summary>
        /// <returns></returns>
        public static TIdentifier DefaultClientID
        {
            get
            {
                return Instance.DefaultClientIdentifier();
            }
        }

        /// <summary>
        /// Return the default value used to represent server identity id
        /// </summary>
        /// <returns></returns>
        public static TIdentifier DefaultServerID
        {
            get
            {
                return Instance.DefaultServerIdentifier();
            }
        }

        public static bool IsServer(TIdentifier identifier)
        {
            return Instance.IsServerIdentifier(identifier);
        }

        public static bool IsClient(TIdentifier identifier)
        {
            return Instance.IsClientIdentifier(identifier);
        }

        public virtual bool IsClientIdentifier(TIdentifier identifier)
        {
            return false;
        }

        public virtual bool IsServerIdentifier(TIdentifier identifier)
        {
            return false;
        }

        public static TIdentifier GenerateClientIdentifier()
        {
            return Instance.GetClientIdentifier();
        }

        public static TIdentifier GenerateServerIdentifier()
        {
            return Instance.GetServerIdentifier();
        }


        /// <summary>
        /// generate the client identity id
        /// </summary>
        /// <returns></returns>
        public virtual TIdentifier DefaultClientIdentifier()
        {
            return default;
        }

        public virtual TIdentifier GetClientIdentifier()
        {
            return default;
        }

        public virtual TIdentifier GetServerIdentifier()
        {
            return default;
        }

        /// <summary>
        /// generate the server identity id
        /// </summary>
        /// <returns></returns>
        public virtual TIdentifier DefaultServerIdentifier()
        {
            return default;
        }
    }
}
