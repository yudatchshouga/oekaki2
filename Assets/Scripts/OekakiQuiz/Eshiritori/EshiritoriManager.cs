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

    private int questionCount;

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
    private void TurnEnd()
    {
        Debug.Log("ターン終了");
        Texture2D texture = CopyTexture(EshiritoriDrawingManager.instance.texture);
        imagePanelController.CreateNewImage(texture);
        imagePanelController.SetText("aaaaaa");
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
                    photonView.RPC("TurnEnd", RpcTarget.All);
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

    private Role GetRole()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber == questionerNumber ? Role.Questioner : Role.Answerer;
    }

    public void SetAnswer(string answer) 
    {
        photonView.RPC("SendAnswer", RpcTarget.All, answer);
    }

    [PunRPC]
    private void SendAnswer(string answer)
    {
        imagePanelController.SetText(answer);
    }
}
