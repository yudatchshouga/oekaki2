using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField] ThemeGenerator themeGenerator;
    public InputField chatInputField;
    public Text chatLogText;
    public ScrollRect chatScrollRect;

    private List<string> chatMessages = new List<string>();

    void Update()
    {
        // エンターキーまたはテンキーのエンターキーが押されたらメッセージを送信
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            //SubmitChatMessage();
            string answer = chatInputField.text;
            if (!string.IsNullOrEmpty(answer))
            {
                // 自分の番号取得
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

                // RPCで送信
                photonView.RPC("SendChatMessage", RpcTarget.All, answer, actorNumber);
                // チャット入力欄をリセット
                chatInputField.text = "";
            }
            // チャット入力欄にフォーカスを移す
            chatInputField.Select();
            chatInputField.ActivateInputField();
        }
    }

    public void SubmitChatMessage()
    {
        string answer = chatInputField.text;
        if (!string.IsNullOrEmpty(answer))
        {
            chatMessages.Add(answer);
            // チャット入力欄をリセット
            chatInputField.text = "";
            chatLogText.text = string.Join("\n", chatMessages.ToArray());
            Canvas.ForceUpdateCanvases();
            // ワンフレーム待つ必要あり？
            chatScrollRect.verticalNormalizedPosition = 0;
        }
    }

    [PunRPC]
    public void SendChatMessage(string message, int actorNumber)
    {
        Debug.Log("SendChatMessage");
        chatMessages.Add(message);
        chatLogText.text = string.Join("\n", chatMessages.ToArray());
        Canvas.ForceUpdateCanvases();
        // ワンフレーム待つ必要あり？
        chatScrollRect.verticalNormalizedPosition = 0;

        //Role role = GameManager.instance.GetRole();
        if (actorNumber == GameManager.instance.questionerNumber)
        {
            // 出題者の場合
            Debug.Log("出題者のメッセージ");
            return;
        }

        // 正誤判定
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.CheckAnswer(message);
        }
    }
}
