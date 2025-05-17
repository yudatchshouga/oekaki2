using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class EshiritoriManager : MonoBehaviourPunCallbacks
{
    public static EshiritoriManager instance;

    private int questionerNumber = 0;
    EshiritoriDotUIManager dotUIManager;

    [SerializeField] Text timerText;
    [SerializeField] TimerController timerController;

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
                    photonView.RPC("TurnStart", RpcTarget.All);
                }
            }
        }
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
}
