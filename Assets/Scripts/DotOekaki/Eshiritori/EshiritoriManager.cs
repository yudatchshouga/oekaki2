using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class EshiritoriManager : MonoBehaviourPunCallbacks
{
    public static EshiritoriManager instance;

    private int questionerNumber;
    EshiritoriDotUIManager dotUIManager;
    Player[] players;

    [SerializeField] Text timerText;

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
        // 設定
        dotUIManager = FindAnyObjectByType<EshiritoriDotUIManager>();
        Debug.Log("オンラインモードで実行");
        questionCount = 100;
        timeLimit = 5;
        players = PhotonNetwork.PlayerList;
        timeRemaining = timeLimit;
        isTimerActive = false;
        isTimeUp = false;

        // ホストが出題者を決定
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetQuestioner", RpcTarget.All, 1);
        }

        Invoke("StartTimer", 1.0f);
        // 出題者の役割を表示
        Role role = GetRole();
        Debug.Log($"あなたの役割: {role}");
        dotUIManager.SetRoleText(role);
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
            timerText.text = $"残り: {timeRemaining.ToString("F0")}秒";
        }
    }

    private void TimeUp()
    {
        Debug.Log("時間切れ!!!");

        isTimerActive = false;
        timeRemaining = timeLimit;
        StartTimer();
        isTimeUp = false;
        // 出題者を変更
        int selectedQuestionerNumber = GetNextQuestioner();
        photonView.RPC("SetQuestioner", RpcTarget.All, selectedQuestionerNumber);
    }
    [PunRPC]
    private void SetQuestioner(int selectedQuestionerNumber)
    {
        Debug.Log("出題者を設定");
        questionCount--;
        EshiritoriDrawingManager.instance.ResetDrawField();
        questionerNumber = selectedQuestionerNumber;
        // テキスト更新
        Role role = GetRole();
        Debug.Log($"あなたの役割: {role}");
        dotUIManager.SetRoleText(role);
        Debug.Log($"出題者: {questionerNumber}");
        if (PhotonNetwork.LocalPlayer.ActorNumber == selectedQuestionerNumber)
        {
            EshiritoriDrawingManager.instance.isDrawable = true;
        }
        else
        {
            EshiritoriDrawingManager.instance.isDrawable = false;
        }
    }

    private int GetNextQuestioner()
    {
        if (questionerNumber == players.Length)
        {
            return 1;
        }
        return questionerNumber + 1;
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
    }

    private string NormalizeString(string input)
    {
        // 前後の空白をトリムし、小文字変換し、全角を半角に変換
        return input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormKC);
    }
}
