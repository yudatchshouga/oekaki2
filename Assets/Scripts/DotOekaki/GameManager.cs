using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("ゲームを開始します。");
            photonView.RPC("StartGameRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartGameRPC()
    {
        Debug.Log("ゲームを開始します。");
    }
}
