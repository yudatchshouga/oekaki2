using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private Player questionner;

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

    // 参加ボタンが押されたときに呼び出される
    public void JoinRandomRoom()
    {
        Debug.Log("ランダムルームに参加します。");
        PhotonNetwork.JoinRandomRoom();
    }

    // === ランダムルームが存在しない場合、新しいルームを作成 ===
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("ランダムルームが存在しないため、新しいルームを作成します。");

        var roomOptions = new RoomOptions
        {
            MaxPlayers = 4, // 最大プレイヤー数4人
            IsOpen = true, // ルームを一般公開する
            IsVisible = true, // ルームがロビーで表示される
        };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    // === ルームに参加したときのコールバック ===
    public override void OnJoinedRoom()
    {
        Debug.Log("ルームに参加しました。");
        // ルーム内のプレイヤー数を取得
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log($"現在のプレイヤー数: {playerCount}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("新しいプレイヤーが参加しました。");
        // 現在の人数を取得
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log($"現在のプレイヤー数: {playerCount}");
    }

    public void OnClickStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("ゲームを開始します。");
            Debug.Log("ゲーム画面に遷移します。");
            photonView.RPC("StartGameRPC", RpcTarget.All);
            // ゲーム開始時に出題者を決定
            Debug.Log("出題者を決定します");
            SelectQuestionner();
        }
    }

    private void SelectQuestionner()
    {
        Player[] players = PhotonNetwork.PlayerList;
        // actorNumber は1始まり
        int selectedActorNumber = Random.Range(0, players.Length) + 1;
        photonView.RPC("SetQuestionner", RpcTarget.All, selectedActorNumber);
    }

    [PunRPC]
    private void StartGameRPC()
    {
        Debug.Log("ゲームを開始します。");
        Debug.Log("ゲーム画面に遷移します。");
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
        Debug.Log("プレイヤーが退出しました。");
    }

    // 退出ボタンが押されたときに呼び出される
    public void OnClickLeave()
    {
        Debug.Log("ルームから退出します。");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("ルームから退出しました。");
    }

    // === 接続失敗時に呼ばれるコールバック ===
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Photon の接続に失敗: {cause}");
    }
}
