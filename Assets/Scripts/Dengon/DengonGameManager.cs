using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DengonGameManager : MonoBehaviourPunCallbacks
{
    public static DengonGameManager instance;

    DengonThemeGenerator themeGenerator;
    DengonUIManager dengonUIManager;
    QuizQuestion currentTheme;

    [SerializeField] GameObject panels;
    [SerializeField] GameObject loadObj;

    [SerializeField] Text correctLabel;
    [SerializeField] Text timerText;
    [SerializeField] Text countText;

    [SerializeField] int questionCount;
    [SerializeField] int questionCountLeft;

    public int timeLimit; // お絵かき時間
    public int answerTime; // 回答時間
    private float timeRemaining;
    private bool isTimerActive;
    private bool isTimeUp;

    List<int> playerOrder = new List<int>(); // プレイヤーの順番を保持するリスト

    [SerializeField] bool isDrawFinished;
    [SerializeField] bool isAnswerFinished;
    [SerializeField] bool isGameFinished;

    [SerializeField] Transform resultList; //結果表示用の親オブジェクト
    [SerializeField] GameObject resultPrefab; //結果表示用のプレハブ
    private List<GameObject> resultPrefabs = new List<GameObject>(); // 結果表示用のプレハブのリスト

    [SerializeField] Transform pictureList; // 結果表示用の画像の親オブジェクト
    [SerializeField] GameObject picturePrefab; // 結果表示用の画像プレハブ
    private List<Texture2D> savedPictures = new List<Texture2D>(); // 保存された画像のリスト

    [SerializeField] GameObject changeSettingButton; // 設定変更ボタン
    [SerializeField] GameObject reuseSettingButton; // 設定再利用ボタン

    private Dictionary<int, int> correctPoints = new Dictionary<int, int>(); // プレイヤーの正解数を保持する辞書
    private Dictionary<int, int> correctedPoints = new Dictionary<int, int>(); // プレイヤーの正解された回数を保持する辞書

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        themeGenerator = FindAnyObjectByType<DengonThemeGenerator>();
        dengonUIManager = FindAnyObjectByType<DengonUIManager>();
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("オンラインモードで実行");
            loadObj.SetActive(true); // ロードオブジェクトを表示
            questionCount = PhotonNetwork.CurrentRoom.PlayerCount - 1; // 描く枚数は参加者数 - 1

            if (PhotonNetwork.IsMasterClient)
            {
                timeLimit = PlayerPrefs.GetInt("DengonTime", 180);
                answerTime = PlayerPrefs.GetInt("DengonAnswerTime", 60);
                photonView.RPC("SyncOption", RpcTarget.All, timeLimit, answerTime); // 制限時間の同期
                DecidePlayerOrder(); // プレイヤーの順番を決定

                changeSettingButton.SetActive(true); // 結果画面の設定変更ボタンを表示
                reuseSettingButton.SetActive(true); // 結果画面の設定再利用ボタンを表示
            }

            timeRemaining = timeLimit;
            isTimerActive = false;
            isTimeUp = false;

            InitializePlayerPoints();
        }
        else
        {
            Debug.Log("オフラインモードで実行");
            timerText.gameObject.SetActive(false);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready"))
        {
            CheckAllPlayerReady();
        }
    }

    private void CheckAllPlayerReady()
    {
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue("Ready", out object isReadyObj))
            {
                // 1人でも未準備のプレイヤーがいる場合はreturn
                if (!(isReadyObj is bool isReady) || !isReady)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        // 全員が準備完了状態ならば、ゲーム開始する
        if (PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // ゲーム終了時の処理
                if (questionCountLeft < 0 && !isGameFinished)
                {
                    Invoke("GameFinished", 2.8f);
                    isGameFinished = true;
                }

                // お絵かきタイム
                if (isTimerActive && timeRemaining > 0 && questionCountLeft > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    photonView.RPC("SyncTimer", RpcTarget.Others, timeRemaining);
                }
                else if (timeRemaining <= 0 && isTimerActive && !isTimeUp)
                {
                    isTimerActive = false;
                    isTimeUp = true;
                    TimeUp();
                }

                // 回答タイム
            }
            timerText.text = $"残り\n{timeRemaining.ToString("F0")} 秒";
            countText.text = $"残り\n{questionCountLeft} 問";
        }
    }

    private void StartGame()
    {
        photonView.RPC("SetQuestioner", RpcTarget.All);
        photonView.RPC("StartTimer", RpcTarget.All); // タイマーを開始
        photonView.RPC("HideLoadObj", RpcTarget.All); // ロードオブジェクトを非表示
    }

    public void RestartGame()
    {
        themeGenerator.GenerateAndShuffleIndex();

        photonView.RPC("SetQuestioner", RpcTarget.All);

        photonView.RPC("SyncSettings", RpcTarget.All);

        photonView.RPC("MoveGamePanel", RpcTarget.All, new Vector2(0, 0));
    }

    // プレイヤーの順番をランダムに決定するメソッド(ホストのみ実行)
    private void DecidePlayerOrder()
    { 
        List<Player> players = new List<Player>(PhotonNetwork.PlayerList);
        List<int> actorNumbers = new List<int>();

        foreach (var p in players)
        {
            actorNumbers.Add(p.ActorNumber);
        }

        for (int i = actorNumbers.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = actorNumbers[i];
            actorNumbers[i] = actorNumbers[j];
            actorNumbers[j] = temp;
        }

        photonView.RPC("ReceivePlayerOrder", RpcTarget.All, actorNumbers.ToArray()); // 順番を全プレイヤーに送信
    }

    [PunRPC]
    private void ReceivePlayerOrder(int[] order)
    { 
        playerOrder.Clear(); // 既存の順番をクリア
        playerOrder = new List<int>(order);
    }

    [PunRPC]
    private void SyncOption(int time1, int time2)
    {
        timeLimit = time1;
        answerTime = time2;
    }

    [PunRPC]
    private void SetQuestioner()
    {
        questionCountLeft--;
        dengonUIManager.Initialize(); // UIリセット
    }

    [PunRPC]
    private void SyncSettings()
    {
        timeRemaining = timeLimit;
        isTimerActive = false;
        isTimeUp = false;

        InitializePlayerPoints();
        DestroyResultPrefabs();

        Invoke("StartTimer", 1.0f);
        isGameFinished = false; // ゲーム開始時にリセット
    }

    [PunRPC]
    private void MoveGamePanel(Vector2 pos)
    {
        panels.transform.localPosition = pos;
    }


    private void TimeUp()
    {
        photonView.RPC("SavedPicture", RpcTarget.All);
        photonView.RPC("SetQuestioner", RpcTarget.All);
    }

    private bool IsCorrectAnswer(string answer)
    {
        foreach (string correctAnswer in currentTheme.answerList)
        {
            if (NormalizeString(answer) == NormalizeString(correctAnswer))
            {
                return true;
            }
        }
        return false;
    }

    public void SubmitAnswer(string answer)
    {
        photonView.RPC("CheckAnswer", RpcTarget.Others, answer, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    private void ShowCorrect()
    {
        isTimerActive = false;
        timeRemaining = timeLimit;
        StartCoroutine(DisplayMessage($"正解！\n「{currentTheme.question}」", 3.0f));
    }

    [PunRPC]
    private void ShowIncorrect()
    {
        isTimerActive = false;
        timeRemaining = timeLimit;
        StartCoroutine(DisplayMessage($"残念...不正解！\n正解は\n「{currentTheme.question}」", 3.0f));
    }

    private IEnumerator DisplayMessage(string message, float duration)
    {
        correctLabel.text = message;
        correctLabel.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        correctLabel.gameObject.SetActive(false);
        StartTimer();
        isTimeUp = false;
    }

    [PunRPC]
    private void StartTimer()
    {
        isTimerActive = true;
    }

    [PunRPC]
    private void SyncTimer(float time)
    {
        timeRemaining = time;
    }

    [PunRPC]
    private void UpdateCurrentTheme(string question)
    {
        if (currentTheme == null)
        {
            currentTheme = new QuizQuestion();
        }
        currentTheme.question = question;
    }

    private string NormalizeString(string input)
    {
        // 前後の空白をトリムし、小文字変換し、全角を半角に変換
        return input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormKC);
    }

    private void DestroyResultPrefabs()
    {
        foreach (GameObject prefab in resultPrefabs)
        {
            Destroy(prefab);
        }
        resultPrefabs.Clear();

        foreach (Transform child in pictureList)
        {
            Destroy(child.gameObject);
        }
        savedPictures.Clear();
    }

    [PunRPC]
    private void HideLoadObj()
    {
        loadObj.SetActive(false);
    }



    // リザルト関連

    private void InitializePlayerPoints()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            correctPoints[player.ActorNumber] = 0;
            correctedPoints[player.ActorNumber] = 0;
        }
    }

    [PunRPC]
    private void AddCorrectPoints(int playerActorNumber)
    {
        if (!correctPoints.ContainsKey(playerActorNumber))
        {
            correctPoints[playerActorNumber] = 0;
        }
        correctPoints[playerActorNumber]++;
    }

    [PunRPC]
    private void AddCorrectedPoints(int playerActorNumber)
    {
        if (!correctedPoints.ContainsKey(playerActorNumber))
        {
            correctedPoints[playerActorNumber] = 0;
        }
        correctedPoints[playerActorNumber]++;
    }

    private int GetCorrectPoints(int playerActorNumber)
    {
        if (correctPoints.TryGetValue(playerActorNumber, out int points))
        {
            return points;
        }
        return 0;
    }

    private int GetCorrectedPoints(int playerActorNumber)
    {
        if (correctedPoints.TryGetValue(playerActorNumber, out int points))
        {
            return points;
        }
        return 0;
    }

    private void DisplayResults()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(resultPrefab, resultList);
            resultPrefabs.Add(entry);
            ResultPrefab resultPref = entry.GetComponent<ResultPrefab>();

            string playerName = player.NickName;
            int correctPoints = GetCorrectPoints(player.ActorNumber);
            int correctedPoints = GetCorrectedPoints(player.ActorNumber);
            int point = (correctPoints + correctedPoints) * 10;

            resultPref.SetResult(playerName, correctPoints, correctedPoints, point);
        }
    }

    [PunRPC]
    private void SyncResults(int[] actorNumbers, int[] correctPointsArray, int[] correctedPointsArray)
    {
        for (int i = 0; i < actorNumbers.Length; i++)
        {
            correctedPoints[actorNumbers[i]] = correctedPointsArray[i];
            correctPoints[actorNumbers[i]] = correctPointsArray[i];
        }
    }

    private void SendResultsToOthers()
    {
        // 辞書を配列に変換
        int[] actorNumbers = new int[correctPoints.Count];
        int[] correctPointsArray = new int[correctPoints.Count];
        int[] correctedPointsArray = new int[correctedPoints.Count];

        int index = 0;
        foreach (var kvp in correctPoints)
        {
            actorNumbers[index] = kvp.Key;
            correctPointsArray[index] = kvp.Value;
            correctedPointsArray[index] = correctedPoints[kvp.Key];
            index++;
        }

        // RPCでデータを送信
        photonView.RPC("SyncResults", RpcTarget.Others, actorNumbers, correctPointsArray, correctedPointsArray);
    }

    [PunRPC]
    private void SavedPicture()
    {
        Texture2D texture = new Texture2D(DengonDrawingManager.instance.texture.width, DengonDrawingManager.instance.texture.height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(DengonDrawingManager.instance.texture.GetPixels());
        texture.Apply();
        savedPictures.Add(texture);
    }

    private void DisplaySavedPictures()
    {
        foreach (Texture2D picture in savedPictures)
        {
            GameObject pictureEntry = Instantiate(picturePrefab, pictureList);
            Transform rawImageTransform = pictureEntry.transform.Find("RawImage");
            RawImage rawImage = rawImageTransform.GetComponent<RawImage>();
            rawImage.texture = picture;
        }
    }

    private void GameFinished()
    {
        Debug.Log("ゲーム終了");
        SendResultsToOthers();
        photonView.RPC("Resultdisplay", RpcTarget.All);
    }

    [PunRPC]
    private void Resultdisplay()
    {
        MoveGamePanel(new Vector2(-2000, 0));
        DisplayResults();
        DisplaySavedPictures();
        dengonUIManager.Initialize();
    }
}
