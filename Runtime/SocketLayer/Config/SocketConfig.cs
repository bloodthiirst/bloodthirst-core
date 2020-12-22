using Bloodthirst.Core.PersistantAsset;
using Sirenix.OdinInspector;
using System.Net;
using UnityEngine;

public class SocketConfig : SingletonScriptableObject<SocketConfig>
{
    [SerializeField]
    private bool isClient;

    [SerializeField]
    private bool isServer;

    /// <summary>
    /// Is the game running as server ?
    /// </summary>
    public bool IsServer
    {
        get
        {
            return isServer;
        }

        set
        {
            isServer = value;
        }
    }

    /// <summary>
    /// is the game running as client ?
    /// </summary>
    public bool IsClient {
        get { 
            return isClient; 
        }

        set { 
            isClient = value; 
        }
    }

    [SerializeField]
    private int packetSize;

    [HideInInspector]
    /// <summary>
    /// packet size
    /// </summary>
    public int PacketSize => packetSize;

    [SerializeField]
    private int worldServerPort;

    [HideInInspector]
    /// <summary>
    /// Socket world server port
    /// </summary>
    public int WorldServerPort => worldServerPort;

    [SerializeField]
    private int ghostServerPort;

    [HideInInspector]
    /// <summary>
    /// Socket ghost server port
    /// </summary>
    public int GhostServerPort => ghostServerPort;

    [BoxGroup("IP")]
    [SerializeField]
    private string serverAddress;

    [HideInInspector]
    /// <summary>
    /// Socket server address
    /// </summary>
    public string ServerAddress => serverAddress;

#if UNITY_EDITOR
    private IPAddress localVal;

    [BoxGroup("IP")]
    [ShowInInspector]
    private bool isValidAddress
    {
        get
        {
            return IPAddress.TryParse(serverAddress, out localVal);
        }
    }
#endif
}
