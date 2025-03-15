using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("�Q�[�����J�n���܂��B");
            photonView.RPC("StartGameRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartGameRPC()
    {
        Debug.Log("�Q�[�����J�n���܂��B");
    }
}
