using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager instance;

    [SerializeField] GameObject startButton;
    [SerializeField] Text playerCountText;
    [SerializeField] InputField passwordInputField;
    Player questionner;
    int maxPlayers = 4;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
            MaxPlayers = maxPlayers, // 最大プレイヤー数4人
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
        playerCountText.text = $"現在のプレイヤー数: {playerCount} / {maxPlayers}";
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

    // 新しいプレイヤーが参加したときのコールバック
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("新しいプレイヤーが参加しました。");
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        playerCountText.text = $"現在のプレイヤー数: {playerCount} / {maxPlayers}";
    }

    // プレイヤーが退出したときのコールバック
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("プレイヤーが退出しました。");
    }

    // ルームから退出する
    public void OnLeaveRoom()
    {
        Debug.Log("ルームから退出します。");
        PhotonNetwork.LeaveRoom();
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

    // パスワードルームに参加する（ルームが存在しなければ作成してから参加する）
    public void OnPasswordRoomButtonClicked()
    {
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = false;

        PhotonNetwork.JoinOrCreateRoom(passwordInputField.text, roomOptions, TypedLobby.Default);
    }
}
