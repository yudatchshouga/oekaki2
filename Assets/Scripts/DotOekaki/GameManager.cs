using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public int questionerNumber;
    [SerializeField] bool randamMode;
    public QuizQuestion currentTheme;
    [SerializeField] public Text correctLabel;

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

    public void SetUpMaster()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // お題決定
            currentTheme = ThemeGenerator.instance.GetRandomTheme();
            // 出題者決定
            int selectedQuestionerNumber = randamMode ? Random.Range(0, PhotonNetwork.PlayerList.Length) + 1 : 1;
            photonView.RPC("SetUpAll", RpcTarget.All, selectedQuestionerNumber, currentTheme.question);
        }
    }

    [PunRPC]
    public void SetUpAll(int selectedQuestionerNumber, string question)
    {
        questionerNumber = selectedQuestionerNumber;
        DotUIManager.instance.SetRoleText(GetRole());
        DotUIManager.instance.SetThemeText(question);
    }

    [PunRPC]
    public void SetNextGame(int selectedQuestionerNumber, string question)
    {
        //ShowCorrectLabel();
        StartCoroutine(ShowCorrectLabel());
        questionerNumber = selectedQuestionerNumber;
        DotUIManager.instance.SetRoleText(GetRole());
        DotUIManager.instance.SetThemeText(question);
    }

    public Role GetRole()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        return actorNumber == questionerNumber ? Role.Questioner : Role.Answerer;
    }

    public bool isDrawable()
    {
        return GetRole() == Role.Questioner;
    }

    // masterのみが呼び出す
    public void CheckAnswer(string answer)
    {
        Debug.Log("CheckAnswer");
        // 答えが一致するかどうかを判定
        foreach (string correctAnswer in currentTheme.answerList)
        {
            if (NormalizeString(answer) == NormalizeString(correctAnswer))
            {
                Debug.Log("正解！");
                // お題決定
                currentTheme = ThemeGenerator.instance.GetRandomTheme();
                // 出題者決定
                Player[] players = PhotonNetwork.PlayerList;
                int selectedQuestionerNumber = randamMode ? Random.Range(0, players.Length) + 1 : 1;
                photonView.RPC("SetNextGame", RpcTarget.All, selectedQuestionerNumber, currentTheme.question);
            }
        }
    }

    private string NormalizeString(string input)
    {
        // 前後の空白をトリムし、小文字変換し、全角を半角に変換
        return input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormKC);
    }


    private IEnumerator ShowCorrectLabel()
    {
        Debug.Log("ShowCorrectLabel");
        correctLabel.text = "正解！";
        correctLabel.gameObject.SetActive(true);
        // 正解のエフェクトを出す
        yield return new WaitForSeconds(2.0f);
        correctLabel.gameObject.SetActive(false);
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    SelectQuestion();
        //}
    }
}
