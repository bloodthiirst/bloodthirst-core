using Bloodthirst.Core.PersistantAsset;
using Sirenix.OdinInspector;
using System.Net;
using UnityEngine;

public class SocketConfig : SingletonScriptableObject<SocketConfig>
{
    [SerializeField]
    private int packetSize = default;

    [HideInInspector]
    /// <summary>
    /// packet size
    /// </summary>
    public int PacketSize => packetSize;

    [SerializeField]
    private int serverPort = default;

    [HideInInspector]
    /// <summary>
    /// Socket server port
    /// </summary>
    public int ServerPort => serverPort;

    [BoxGroup("IP")]
    [SerializeField]
    private string serverAddress = default;

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
