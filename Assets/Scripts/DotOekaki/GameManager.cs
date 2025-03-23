using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    ThemeGenerator themeGenerator;
    DotUIManager dotUIManager;
    QuizQuestion currentTheme;
    Player[] players;
    [SerializeField] Text correctLabel;
    [SerializeField] bool randamMode;
    public Role role;

    public int questionerNumber;

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
        players = PhotonNetwork.PlayerList;
        Setup();
    }

    private void Setup()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // お題と出題者の決定
            int selectedQuestionerNumber = randamMode ? Random.Range(0, players.Length) + 1 : 1;
            photonView.RPC("SetQuestioner", RpcTarget.All, selectedQuestionerNumber);
        }
    }

    [PunRPC]
    private void SetQuestioner(int selectedQuestionerNumber)
    {
        questionerNumber = selectedQuestionerNumber;
        role = PhotonNetwork.LocalPlayer.ActorNumber == selectedQuestionerNumber ? Role.Questioner : Role.Answerer;
        currentTheme = themeGenerator.GetRandomTheme();
        Invoke("UpdateUI", 0.5f);
    }

    [PunRPC]
    private void SetNextGame(int selectedQuestionerNumber)
    {
        questionerNumber = selectedQuestionerNumber;
        role = PhotonNetwork.LocalPlayer.ActorNumber == selectedQuestionerNumber ? Role.Questioner : Role.Answerer;
        if (role == Role.Questioner)
        {
            currentTheme = themeGenerator.GetRandomTheme();
        }
        StartCoroutine(ShowCorrectLabel());
    }

    // 出題者のみが正誤判定を行う
    public void CheckAnswer(string answer)
    {
        if (role != Role.Questioner)
        {
            return;
        }

        Debug.Log("CheckAnswer");
        foreach (string correctAnswer in currentTheme.answerList)
        {
            if (NormalizeString(answer) == NormalizeString(correctAnswer))
            {
                // お題と出題者の再設定
                int selectedQuestionerNumber = randamMode ? Random.Range(0, players.Length) + 1 : 1;
                photonView.RPC("SetNextGame", RpcTarget.All, selectedQuestionerNumber);
                return;
            }
        }
    }

    private IEnumerator ShowCorrectLabel()
    {
        correctLabel.text = "正解！";
        correctLabel.gameObject.SetActive(true);
        // 正解のエフェクトを出す
        yield return new WaitForSeconds(2.0f);
        correctLabel.gameObject.SetActive(false);
        UpdateUI();
    }

    private void UpdateUI()
    {
        dotUIManager.SetRoleText(role);
        dotUIManager.SetThemeText(role, currentTheme.question);
    }


    private string NormalizeString(string input)
    {
        // 前後の空白をトリムし、小文字変換し、全角を半角に変換
        return input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormKC);
    }

    public bool IsDrawable()
    {
        return role == Role.Questioner;
    }
}
