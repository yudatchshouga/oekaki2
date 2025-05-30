using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    private int questionerNumber;
    public int QuestionerNumber => questionerNumber;

    ThemeGenerator themeGenerator;
    DotUIManager dotUIManager;
    QuizQuestion currentTheme;

    [SerializeField] GameObject panels;

    [SerializeField] Text correctLabel;
    [SerializeField] Text timerText;
    [SerializeField] Text countText;
    [SerializeField] bool randomMode;

    public int questionCount;
    public int questionCountLeft;

    public int timeLimit;
    private float timeRemaining;
    private bool isTimerActive;
    private bool isTimeUp;

    [SerializeField] bool isFinished;

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
        themeGenerator = FindAnyObjectByType<ThemeGenerator>();
        dotUIManager = FindAnyObjectByType<DotUIManager>();
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("オンラインモードで実行");

            // ホストがお題と出題者を決定し、設定を同期する
            if (PhotonNetwork.IsMasterClient)
            {
                questionCount = PlayerPrefs.GetInt("QuestionCount", 5);
                timeLimit = PlayerPrefs.GetInt("LimitTime", 180);

                photonView.RPC("SyncOption", RpcTarget.All, randomMode, questionCount, timeLimit);

                int selectedQuestionerNumber = randomMode ? Random.Range(0, PhotonNetwork.PlayerList.Length) + 1 : 1;
                photonView.RPC("SetQuestioner", RpcTarget.All, selectedQuestionerNumber);

                changeSettingButton.SetActive(true); // 設定変更ボタンを表示
                reuseSettingButton.SetActive(true); // 設定再利用ボタンを表示
            }

            timeRemaining = timeLimit;
            isTimerActive = false;
            isTimeUp = false;

            InitializePlayerPoints();

            Invoke("StartTimer", 1.0f);
            Invoke("UpdateText", 1.0f);
        }
        else
        {
            Debug.Log("オフラインモードで実行");
            timerText.gameObject.SetActive(false);
        }
    }
    

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (questionCountLeft < 0 && !isFinished)
                {
                    Invoke("GameFinished", 2.8f);
                    isFinished = true;
                }

                if (isTimerActive && timeRemaining > 0 && questionCountLeft >= 0)
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
            }
            timerText.text = $"残り\n{timeRemaining.ToString("F0")} 秒";
            countText.text = $"残り\n{questionCountLeft} 問";
        }
    }

    public void StartGame()
    {
        questionCount = PlayerPrefs.GetInt("QuestionCount", 5);
        timeLimit = PlayerPrefs.GetInt("LimitTime", 180);

        photonView.RPC("SyncOption", RpcTarget.All, randomMode, questionCount, timeLimit);

        int selectedQuestionerNumber = randomMode ? Random.Range(0, PhotonNetwork.PlayerList.Length) + 1 : 1;
        photonView.RPC("SetQuestioner", RpcTarget.All, selectedQuestionerNumber);

        photonView.RPC("SyncSettings", RpcTarget.All);

        photonView.RPC("MoveGamePanel", RpcTarget.All);
    }

    public void RestartGame()
    {
        photonView.RPC("SyncOption", RpcTarget.All, randomMode, questionCount, timeLimit);

        int selectedQuestionerNumber = randomMode ? Random.Range(0, PhotonNetwork.PlayerList.Length) + 1 : 1;
        photonView.RPC("SetQuestioner", RpcTarget.All, selectedQuestionerNumber);

        photonView.RPC("SyncSettings", RpcTarget.All);

        photonView.RPC("MoveGamePanel", RpcTarget.All);
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
        Invoke("UpdateText", 1.0f);
        isFinished = false; // ゲーム開始時にリセット
    }

    [PunRPC]
    private void MoveGamePanel()
    {
        panels.transform.localPosition = new Vector2(0, 0);
    }


    private void TimeUp()
    {
        photonView.RPC("SavedPicture", RpcTarget.All);
        photonView.RPC("ShowIncorrect", RpcTarget.All);
        int selectedQuestionerNumber = randomMode ? Random.Range(0, PhotonNetwork.PlayerList.Length) + 1 : 1;
        photonView.RPC("SetQuestioner", RpcTarget.All, selectedQuestionerNumber);
    }

    // randomModeとquestionCountとtimeLimitをホストと同期する
    [PunRPC]
    private void SyncOption(bool random, int count, int time)
    {
        randomMode = random;
        questionCountLeft = count;
        timeLimit = time;
    }

    [PunRPC]
    private void SetQuestioner(int selectedQuestionerNumber)
    {
        questionCountLeft--;
        DrawingManager.instance.ResetDrawField();
        DrawingManager.instance.currentMode = DrawingManager.ToolMode.Pen; // 描き手のモードに戻す
        questionerNumber = selectedQuestionerNumber;
        if (PhotonNetwork.LocalPlayer.ActorNumber == selectedQuestionerNumber)
        {
            currentTheme = themeGenerator.GetRandomTheme();
            photonView.RPC("UpdateCurrentTheme", RpcTarget.Others, currentTheme.question);
            DrawingManager.instance.isDrawable = true;
        }
        else
        {
            DrawingManager.instance.isDrawable = false;
        }
    }

    // 出題者のみが正誤判定を行う
    [PunRPC]
    private void CheckAnswer(string answer, int actorNumber)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == questionerNumber && IsCorrectAnswer(answer))
        {
            photonView.RPC("ShowCorrect", RpcTarget.All);
            photonView.RPC("AddCorrectPoints", RpcTarget.MasterClient, actorNumber); // 正解したプレイヤーの正解数を加算
            photonView.RPC("AddCorrectedPoints", RpcTarget.MasterClient, questionerNumber); // 出題者の正解された回数を加算
            photonView.RPC("SavedPicture", RpcTarget.All); // 正解時に画像を保存

            // TODO:残り秒数などでポイント増やすか検討（実際にプレイした所感で決めたい）

            // お題と出題者の再設定
            int selectedQuestionerNumber = randomMode ? Random.Range(0, PhotonNetwork.PlayerList.Length) + 1 : 1;
            photonView.RPC("SetQuestioner", RpcTarget.All, selectedQuestionerNumber);
        }
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
        StartCoroutine(DisplayMessage("正解！", 3.0f));
    }

    [PunRPC]
    private void ShowIncorrect()
    {
        isTimerActive = false;
        timeRemaining = timeLimit;
        StartCoroutine(DisplayMessage($"残念...不正解！\n正解は\n「{currentTheme.question}」\nだったよ！", 3.0f));
    }

    private IEnumerator DisplayMessage(string message, float duration)
    {
        correctLabel.text = message;
        correctLabel.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        correctLabel.gameObject.SetActive(false);
        UpdateText();
        StartTimer();
        isTimeUp = false;
    }

    private void UpdateText()
    {
        dotUIManager.SetRoleText(GetRole());
        dotUIManager.SetThemeText(GetRole(), currentTheme.question);
    }

    private Role GetRole()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber == questionerNumber ? Role.Questioner : Role.Answerer;
    }

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
        if(!correctPoints.ContainsKey(playerActorNumber))
        {
            correctPoints[playerActorNumber] = 0;
        }
        correctPoints[playerActorNumber] ++;
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
        Texture2D texture = new Texture2D(DrawingManager.instance.texture.width, DrawingManager.instance.texture.height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(DrawingManager.instance.texture.GetPixels());
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
        panels.transform.localPosition = new Vector2(-2000, 0);
        DisplayResults();
        DisplaySavedPictures();
    }    
}
