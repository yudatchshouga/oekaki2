using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Photon に接続中...");
    }

    // === 接続成功時に呼ばれるコールバック ===
    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon に接続成功！");
    }

    // === 接続失敗時に呼ばれるコールバック ===
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Photon の接続に失敗: {cause}");
    }
}
