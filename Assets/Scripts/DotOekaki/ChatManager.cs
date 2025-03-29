using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField] InputField chatInputField;
    [SerializeField] Text chatLogText;
    [SerializeField] ScrollRect chatScrollRect;

    List<string> chatMessages = new List<string>();

    void Update()
    {
        // エンターキーまたはテンキーのエンターキーが押されたらメッセージを送信
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            string answer = chatInputField.text;
            if (!string.IsNullOrEmpty(answer))
            {
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber; // 自分の番号取得
                string senderName = PhotonNetwork.LocalPlayer.NickName; // 自分の名前取得
                photonView.RPC("SendChatMessage", RpcTarget.All, answer, senderName);
                chatInputField.text = ""; // チャット入力欄をリセット

                // 出題者の場合は回答を提出
                if (PhotonNetwork.LocalPlayer.ActorNumber != GameManager.instance.QuestionerNumber)
                {
                    GameManager.instance.SubmitAnswer(answer);
                }
            }
            // チャット入力欄にフォーカスを移す
            chatInputField.Select();
            chatInputField.ActivateInputField();
        }
    }

    [PunRPC]
    private void SendChatMessage(string message, string senderName)
    {
        string messageWithSender = senderName + ": " + message;
        chatMessages.Add(messageWithSender);
        chatLogText.text = string.Join("\n", chatMessages.ToArray());
        Canvas.ForceUpdateCanvases(); // ワンフレーム待つ必要あり？
        chatScrollRect.verticalNormalizedPosition = 0;
    }
}
