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
    public bool randamMode = true;
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

    public void SelectQuestioner()
    {
        Debug.Log("SelectQuestioner");
        if (PhotonNetwork.IsMasterClient)
        {
            Player[] players = PhotonNetwork.PlayerList;
            // actorNumber は1始まり
            int selectedActorNumber = randamMode ? Random.Range(0, players.Length) + 1 : 1;
            photonView.RPC("SetQuestionner", RpcTarget.All, selectedActorNumber);
        }
    }
    [PunRPC]
    public void SetQuestionner(int actorNumber)
    {
        Debug.Log("SetQuestionner");
        questionerNumber = actorNumber;
        DotUIManager.instance.SetRoleText(GetRole());
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

    public void SelectQuestion()
    {
        Debug.Log("SelectQuestion");
        // お題決定
        currentTheme = ThemeGenerator.instance.GetRandomTheme();
        // 出題者決定
        SelectQuestioner();
        // UI反映
        DotUIManager.instance.SetThemeText(currentTheme.question);
    }

    public void CheckAnswer(string answer)
    {
        Debug.Log("CheckAnswer");
        // 答えが一致するかどうかを判定
        foreach (string correctAnswer in currentTheme.answerList)
        {
            if (NormalizeString(answer) == NormalizeString(correctAnswer))
            {
                photonView.RPC("receiveCorrectAnswer", RpcTarget.All);
            }
        }
    }

    private string NormalizeString(string input)
    {
        // 前後の空白をトリムし、小文字変換し、全角を半角に変換
        return input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormKC);
    }

    [PunRPC]
    private void receiveCorrectAnswer()
    {
        Debug.Log("receiveCorrectAnswer"); 
        StartCoroutine(ShowCorrectLabel());
    }

    private IEnumerator ShowCorrectLabel()
    {
        Debug.Log("ShowCorrectLabel");
        correctLabel.text = "正解！";
        correctLabel.gameObject.SetActive(true);
        // 正解のエフェクトを出す
        yield return new WaitForSeconds(2.0f);
        correctLabel.gameObject.SetActive(false);
        SelectQuestion();
    }
}
