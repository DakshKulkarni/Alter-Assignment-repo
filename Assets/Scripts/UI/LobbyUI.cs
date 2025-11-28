using Mirror;
using UnityEngine;
public class LobbyUI : MonoBehaviour
{
    // For testing on the same PC 
    [Header("PC / Editor")]
    public string pcAddress = "10.1.163.94";

    // For Android builds
    [Header("Android Client")]
    public string androidHostIp = "10.1.163.94";
    public void OnClickHost()
    {
        Debug.Log("[Lobby] Starting Host");
        NetworkManager.singleton.StartHost();
    }
    public void OnClickJoin()
    {
        string addr;

#if UNITY_ANDROID && !UNITY_EDITOR
        // On a real Android device, connect to the IP of the host device
        addr = androidHostIp;
#else
        addr = pcAddress;
#endif
        Debug.Log("[Lobby] Joining host at " + addr);
        NetworkManager.singleton.networkAddress = addr;
        NetworkManager.singleton.StartClient();
    }
    public void OnClickQuit()
    {
        Application.Quit();
    }
}
