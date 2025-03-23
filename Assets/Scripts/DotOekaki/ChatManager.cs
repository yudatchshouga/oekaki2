using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public InputField chatInputField;
    public Text chatLogText;
    public ScrollRect chatScrollRect;

    private List<string> chatMessages = new List<string>();

    void Update()
    {
        // エンターキーまたはテンキーのエンターキーが押されたらメッセージを送信
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            string answer = chatInputField.text;
            if (!string.IsNullOrEmpty(answer))
            {
                // 自分の番号取得
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

                // RPCで送信
                photonView.RPC("SendChatMessage", RpcTarget.All, answer);
                // チャット入力欄をリセット
                chatInputField.text = "";
            }
            // チャット入力欄にフォーカスを移す
            chatInputField.Select();
            chatInputField.ActivateInputField();
        }
    }

    [PunRPC]
    private void SendChatMessage(string message)
    {
        Debug.Log("SendChatMessage");
        UpdateChatField(message);

        // 正誤判定
        GameManager.instance.CheckAnswer(message);
    }

    private void UpdateChatField(string message)
    {
        chatMessages.Add(message);
        chatLogText.text = string.Join("\n", chatMessages.ToArray());
        Canvas.ForceUpdateCanvases(); // ワンフレーム待つ必要あり？
        chatScrollRect.verticalNormalizedPosition = 0;
    }
}
