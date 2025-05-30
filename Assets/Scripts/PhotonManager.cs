using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager instance;

    [SerializeField] GameObject startButton;
    [SerializeField] Text playerCountText;
    [SerializeField] Text roomNameText;
    [SerializeField] Text joinedPlayerText;

    [SerializeField] int previousPlayerCount = 0; // 前回のプレイヤー数を記録するための変数

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Photon に接続中...");
    }

    // ルームを監視し、変更があった場合は更新する
    void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            int currentPlayerCount = PhotonNetwork.PlayerList.Length;
            if (currentPlayerCount != previousPlayerCount)
            {
                previousPlayerCount = currentPlayerCount;
                UpdatePlayerListUI();
            }
        }
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
            MaxPlayers = 2, // 最大プレイヤー数4人
            IsOpen = true, // ルームを一般公開する
            IsVisible = true, // ルームがロビーで表示される
        };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    // === ルームに参加したときのコールバック ===
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("PlayerName", $"Player{Random.Range(1000, 9999)}");

        roomNameText.text = $"ルーム名：{PhotonNetwork.CurrentRoom.Name}";
        UpdatePlayerListUI();

        // ホストのみゲームルールを選んで開始することができる
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        { 
            startButton.SetActive(false);
        }
    }

    public void UpdatePlayerListUI()
    {
        playerCountText.text = $"現在のプレイヤー数: {PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
        joinedPlayerText.gameObject.SetActive(false); // 一度非表示にする
        joinedPlayerText.gameObject.SetActive(true);  // 再表示する
        joinedPlayerText.text = string.Join("\n", GetPlayerNameList());
    }

    private string[] GetPlayerNameList()
    {
        Player[] players = PhotonNetwork.PlayerList;
        string[] playerNameList = new string[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerNameList[i] = players[i].NickName;
        }
        return playerNameList;
    }

    // 新しいプレイヤーが参加したときのコールバック
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName}が参加しました。");

        UpdatePlayerListUI();

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            startButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            startButton.GetComponent<Button>().interactable = false;
        }
    }

    // プレイヤーが退出したときのコールバック
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName}がルームから退出しました。");

        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (currentSceneName == "Title")
        {
            UpdatePlayerListUI();
        }
        else if (currentSceneName == "OekakiQuiz")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // 誰かがルームを抜けてしまった場合、ルームを解散してタイトル画面に戻る
                LeaveRoomAndReturn();
                PhotonNetwork.LoadLevel("Title");
            }
        }
    }

    // ルームを全プレイヤーで破棄する
    private void LeaveRoomAndReturn()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false; // ルームを閉じる
        PhotonNetwork.CurrentRoom.IsVisible = false; // ルームを非表示にする
        PhotonNetwork.LeaveRoom(); // ルームを離れる
    }

    // ルームから退出する
    public void OnLeaveRoom()
    {
        Debug.Log("ルームから退出します。");

        if (PhotonNetwork.IsMasterClient)
        {
            // ホストが退出する場合、ルームを解散する
            LeaveRoomAndReturn();
            // シーンを切り替える
            PhotonNetwork.LoadLevel("Title");
        }
        else
        {
            // ゲストが退出する場合はそのまま退出
            PhotonNetwork.LeaveRoom();
        }
    }

    // ルームから退出する(タイトルシーンに戻る)
    public void OnLeaveRoomAndDestroy()
    {
        Debug.Log("ルームから退出します。");
        PhotonNetwork.LeaveRoom();
        Destroy(gameObject);
        SceneController.instance.LoadScene("Title");
    }

    // === ルームから退出したときのコールバック ===
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
