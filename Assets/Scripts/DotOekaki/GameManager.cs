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
    Player[] players;

    [SerializeField] Text correctLabel;
    [SerializeField] Text timerText;
    [SerializeField] bool randomMode;

    private int questionCount;

    public float timeLimit;
    private float timeRemaining;
    private bool isTimerActive;
    private bool isTimeUp;

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
            randomMode = PlayerPrefs.GetInt("Random", 1) == 1;
            questionCount = PlayerPrefs.GetInt("QuestionCount", 5);
            timeLimit = PlayerPrefs.GetInt("LimitTime", 180);
            players = PhotonNetwork.PlayerList;
            timeRemaining = timeLimit;
            isTimerActive = false;
            isTimeUp = false;

            // ホストがお題と出題者を決定する
            if (PhotonNetwork.IsMasterClient)
            {
                int selectedQuestionerNumber = randomMode ? Random.Range(0, players.Length) + 1 : 1;
                photonView.RPC("SetQuestioner", RpcTarget.All, selectedQuestionerNumber);
            }

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
                if (questionCount <= 0)
                {
                    Debug.Log("ゲーム終了");
                }

                if (isTimerActive && timeRemaining > 0)
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
            timerText.text = $"残り\n{timeRemaining.ToString("F0")}秒";
        }
    }

    private void TimeUp()
    {
        photonView.RPC("ShowIncorrect", RpcTarget.All);
        int selectedQuestionerNumber = randomMode ? Random.Range(0, players.Length) + 1 : 1;
        photonView.RPC("SetQuestioner", RpcTarget.All, selectedQuestionerNumber);
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
            int selectedQuestionerNumber = randomMode ? Random.Range(0, players.Length) + 1 : 1;
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
}
