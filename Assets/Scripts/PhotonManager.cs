using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private Player questionner;

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

    // �Q���{�^���������ꂽ�Ƃ��ɌĂяo�����
    public void JoinRandomRoom()
    {
        Debug.Log("�����_�����[���ɎQ�����܂��B");
        PhotonNetwork.JoinRandomRoom();
    }

    // === �����_�����[�������݂��Ȃ��ꍇ�A�V�������[�����쐬 ===
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("�����_�����[�������݂��Ȃ����߁A�V�������[�����쐬���܂��B");

        var roomOptions = new RoomOptions
        {
            MaxPlayers = 4, // �ő�v���C���[��4�l
            IsOpen = true, // ���[������ʌ��J����
            IsVisible = true, // ���[�������r�[�ŕ\�������
        };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    // === ���[���ɎQ�������Ƃ��̃R�[���o�b�N ===
    public override void OnJoinedRoom()
    {
        Debug.Log("���[���ɎQ�����܂����B");
        // ���[�����̃v���C���[�����擾
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log($"���݂̃v���C���[��: {playerCount}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("�V�����v���C���[���Q�����܂����B");
        // ���݂̐l�����擾
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log($"���݂̃v���C���[��: {playerCount}");
    }

    public void OnClickStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("�Q�[�����J�n���܂��B");
            Debug.Log("�Q�[����ʂɑJ�ڂ��܂��B");
            photonView.RPC("StartGameRPC", RpcTarget.All);
            // �Q�[���J�n���ɏo��҂�����
            Debug.Log("�o��҂����肵�܂�");
            SelectQuestionner();
        }
    }

    private void SelectQuestionner()
    {
        Player[] players = PhotonNetwork.PlayerList;
        // actorNumber ��1�n�܂�
        int selectedActorNumber = Random.Range(0, players.Length) + 1;
        photonView.RPC("SetQuestionner", RpcTarget.All, selectedActorNumber);
    }

    [PunRPC]
    private void StartGameRPC()
    {
        Debug.Log("�Q�[�����J�n���܂��B");
        Debug.Log("�Q�[����ʂɑJ�ڂ��܂��B");
        SceneController.LoadScene("DotOekaki");
    }

    [PunRPC]
    private void SetQuestionner(int actorNumber)
    {
        if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            PlayerPrefs.SetString("role", "questionner");
        }
        else
        {
            PlayerPrefs.SetString("role", "answerer");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("�v���C���[���ޏo���܂����B");
    }

    // �ޏo�{�^���������ꂽ�Ƃ��ɌĂяo�����
    public void OnClickLeave()
    {
        Debug.Log("���[������ޏo���܂��B");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("���[������ޏo���܂����B");
    }

    // === �ڑ����s���ɌĂ΂��R�[���o�b�N ===
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Photon �̐ڑ��Ɏ��s: {cause}");
    }
}
