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
        Debug.Log("�����_�����[���ɎQ�����܂��B");
        PhotonNetwork.JoinRandomRoom();
    }

    // === �����_�����[�������݂��Ȃ��ꍇ�A�V�������[�����쐬 ===
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("�����_�����[�������݂��Ȃ����߁A�V�������[�����쐬���܂��B");
        PhotonNetwork.CreateRoom(null);
    }

    // === ���[���ɎQ�������Ƃ��̃R�[���o�b�N ===
    public override void OnJoinedRoom()
    {
        Debug.Log("���[���ɎQ�����܂����B");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("�V�����v���C���[���Q�����܂����B");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("�v���C���[���ޏo���܂����B");
    }

    // === �ڑ����s���ɌĂ΂��R�[���o�b�N ===
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Photon �̐ڑ��Ɏ��s: {cause}");
    }
}
