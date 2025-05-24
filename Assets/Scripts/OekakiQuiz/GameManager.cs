using Photon.Pun;
using Photon.Realtime;
using System.Collections;
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

    private int questionCount;

    public int timeLimit;
    private float timeRemaining;
    private bool isTimerActive;
    private bool isTimeUp;

    private bool isFinished = false;

    [SerializeField] private Transform resultList; //結果表示用の親オブジェクト
    [SerializeField] private GameObject resultPrefab; //結果表示用のプレハブ

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
                randomMode = PlayerPrefs.GetInt("Random", 1) == 1;
                questionCount = PlayerPrefs.GetInt("QuestionCount", 5);
                timeLimit = PlayerPrefs.GetInt("LimitTime", 180);
                photonView.RPC("SyncOption", RpcTarget.All, randomMode, questionCount, timeLimit);

                int selectedQuestionerNumber = randomMode ? Random.Range(0, PhotonNetwork.PlayerList.Length) + 1 : 1;
                photonView.RPC("SetQuestioner", RpcTarget.All, selectedQuestionerNumber);
            }

            timeRemaining = timeLimit;
            isTimerActive = false;
            isTimeUp = false;

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
                if (questionCount < 0 && !isFinished)
                {
                    Debug.Log("ゲーム終了");
                    photonView.RPC("Resultdisplay", RpcTarget.All);
                    isFinished = true;
                }

                if (isTimerActive && timeRemaining > 0 && questionCount >= 0)
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
            countText.text = $"残り\n{questionCount} 問";
        }
    }

    private void TimeUp()
    {
        photonView.RPC("ShowIncorrect", RpcTarget.All);
        int selectedQuestionerNumber = randomMode ? Random.Range(0, PhotonNetwork.PlayerList.Length) + 1 : 1;
        photonView.RPC("SetQuestioner", RpcTarget.All, selectedQuestionerNumber);
    }

    // randomModeとquestionCountとtimeLimitをホストと同期する
    [PunRPC]
    private void SyncOption(bool random, int count, int time)
    {
        randomMode = random;
        questionCount = count;
        timeLimit = time;
    }

    [PunRPC]
    private void SetQuestioner(int selectedQuestionerNumber)
    {
        questionCount--;
        DrawingManager.instance.ResetDrawField();
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
    private void CheckAnswer(string answer)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == questionerNumber && IsCorrectAnswer(answer))
        {
            photonView.RPC("ShowCorrect", RpcTarget.All);
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
        photonView.RPC("CheckAnswer", RpcTarget.Others, answer);
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

    // リザルト関連

    public void SetCorrectAnswers(int correctAnswers)
    {
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
        {
            {"CorrectAnswers", correctAnswers }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    public int GetCorrectAnswers(Player player)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("CorrectAnswers", out object value))
        {
            return (int)value;
        }
        return 0;
    }

    private void DisplayResults()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        { 
            GameObject entry = Instantiate(resultPrefab, resultList);
            ResultPrefab resultPref = entry.GetComponent<ResultPrefab>();

            string playerName = player.NickName;
            int correctAnswers = GetCorrectAnswers(player);
            int correctedAnswers = 0; // TODO: 正解された回数を取得する方法を実装する
            int point = 0; // TODO: ポイントを取得する方法を実装する

            resultPref.SetResult(playerName, correctAnswers, correctedAnswers, point);
        }
    }

    [PunRPC]
    private void Resultdisplay()
    { 
        panels.transform.localPosition = new Vector2(-2000, 0);
        DisplayResults();
    }
}
