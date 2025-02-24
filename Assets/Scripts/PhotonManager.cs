using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Photon �ɐڑ���...");
    }

    // === �ڑ��������ɌĂ΂��R�[���o�b�N ===
    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon �ɐڑ������I");
    }

    // === �ڑ����s���ɌĂ΂��R�[���o�b�N ===
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Photon �̐ڑ��Ɏ��s: {cause}");
    }
}
