using NUnit.Framework;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class EshiritoriManager : MonoBehaviourPunCallbacks
{
    public static EshiritoriManager instance;

    private int questionerNumber = 0;
    EshiritoriDotUIManager dotUIManager;

    [SerializeField] TimerController timerController;
    [SerializeField] ImagePanelController imagePanelController;
    [SerializeField] AnsweView answerView;

    private int questionCount;
    private List<string> answers = new List<string>();

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
        questionCount = 100;
        answerView.OnSubmitAnswer = SetAnswer;

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("TurnStart", RpcTarget.All);
        }
    }

    [PunRPC]
    private void TurnStart()
    {
        Debug.Log("ターン開始");
        // タイマーをリセット
        timerController.ResetTimer();
        // 出題者決定
        questionerNumber = GetNextQuestioner();
        // 出題者の役割を表示
        Role role = GetRole();
        Debug.Log($"あなたの役割: {role}");
        dotUIManager.SetRoleText(role);
        EshiritoriDrawingManager.instance.ResetDrawField();
        if (PhotonNetwork.LocalPlayer.ActorNumber == questionerNumber)
        {
            EshiritoriDrawingManager.instance.isDrawable = true;
        }
        else
        {
            EshiritoriDrawingManager.instance.isDrawable = false;
        }
    }

    [PunRPC]
    private void TurnEnd(int questionerNumber)
    {
        Debug.Log("ターン終了");
        Texture2D texture = CopyTexture(EshiritoriDrawingManager.instance.texture);
        imagePanelController.CreateNewImage(texture);
        imagePanelController.SetText("aaaaaa");
        // 回答パネル表示
        if (IsQuestioner())
        {
            answerView.OpenAnswerPanel();
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
                if (timerController.GetRemainingTime() <= 0)
                {
                    photonView.RPC("TurnEnd", RpcTarget.All, questionerNumber);
                    photonView.RPC("TurnStart", RpcTarget.All);
                }
            }
        }
    }

    private Texture2D CopyTexture(Texture2D texture)
    {
        Texture2D copy = new Texture2D(texture.width, texture.height, texture.format, false);
        copy.SetPixels(texture.GetPixels());
        copy.Apply();
        return copy;
    }

    private int GetNextQuestioner()
    {
        if (questionerNumber == 0)
        {
            return 1;
        }
        if (questionerNumber == PhotonNetwork.PlayerList.Length)
        {
            return 1;
        }
        return questionerNumber + 1;
    }

    // 前の出題者の番号
    public int GetPreviousQuestionerNumber()
    {
        if (questionerNumber == 1)
        {
            return PhotonNetwork.PlayerList.Length;
        }
        return questionerNumber - 1;
    }

    private Role GetRole()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber == questionerNumber ? Role.Questioner : Role.Answerer;
    }

    public void SetAnswer(string answer) 
    {
        int senderNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        photonView.RPC("SendAnswer", RpcTarget.All, answer, senderNumber);
    }

    [PunRPC]
    private void SendAnswer(string answer, int senderNumber)
    {
        answers.Add(answer);
        bool isSender = senderNumber == PhotonNetwork.LocalPlayer.ActorNumber;
        string displayAnswer = isSender ? answer : new string('●', answer.Length);
        imagePanelController.SetText(displayAnswer);
    }

    public void OnClickTurnEnd()
    {
        photonView.RPC("TurnEnd", RpcTarget.All, questionerNumber);
        photonView.RPC("TurnStart", RpcTarget.All);
    }

    private bool IsQuestioner()
    {
        return questionerNumber == PhotonNetwork.LocalPlayer.ActorNumber;
    }
}
