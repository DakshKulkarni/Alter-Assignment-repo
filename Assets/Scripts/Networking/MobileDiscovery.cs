using Mirror.Discovery;
using UnityEngine;
using Mirror;
public class MobileDiscovery : MonoBehaviour
{
    public NetworkDiscovery networkDiscovery;
    private void Awake()
    {
        if (networkDiscovery == null)
            networkDiscovery = GetComponent<NetworkDiscovery>();
    }
    // called by lobbyui after StartHost
    public void StartAdvertise()
    {
        networkDiscovery.AdvertiseServer();
    }
    // called by lobbyui when player presses join
    public void StartSearch()
    {
        networkDiscovery.StartDiscovery();
    }
    public void OnServerFound(ServerResponse info)
    {
        Debug.Log("[Discovery] Found server at: " + info.EndPoint);
        networkDiscovery.StopDiscovery();
        NetworkManager.singleton.networkAddress =
            info.EndPoint.Address.ToString();
        NetworkManager.singleton.StartClient();
    }
}
