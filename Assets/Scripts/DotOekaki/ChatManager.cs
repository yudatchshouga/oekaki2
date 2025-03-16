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
            //SubmitChatMessage();
            string message = chatInputField.text;
            if (!string.IsNullOrEmpty(message))
            {
                chatMessages.Add(message);
                // チャット入力欄をリセット
                chatInputField.text = "";
                chatLogText.text = string.Join("\n", chatMessages.ToArray());
                Canvas.ForceUpdateCanvases();
                // ワンフレーム待つ必要あり？
                chatScrollRect.verticalNormalizedPosition = 0;
            }
            // チャット入力欄にフォーカスを移す
            chatInputField.Select();
            chatInputField.ActivateInputField();

            // RPCで送信
            photonView.RPC("SendChatMessage", RpcTarget.Others, message);
        }
    }

    public void SubmitChatMessage()
    {
        string message = chatInputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            chatMessages.Add(message);
            // チャット入力欄をリセット
            chatInputField.text = "";
            chatLogText.text = string.Join("\n", chatMessages.ToArray());
            Canvas.ForceUpdateCanvases();
            // ワンフレーム待つ必要あり？
            chatScrollRect.verticalNormalizedPosition = 0;
        }
    }

    [PunRPC]
    public void SendChatMessage(string message)
    {
        chatMessages.Add(message);
        chatLogText.text = string.Join("\n", chatMessages.ToArray());
        Canvas.ForceUpdateCanvases();
        // ワンフレーム待つ必要あり？
        chatScrollRect.verticalNormalizedPosition = 0;

        // 正誤判定
        DrawingManager.instance.CheckAnswer(message);
    }

    //private void UpdateChatLog()
    //{
    //    chatLogText.text = string.Join("\n", chatMessages.ToArray());　// チャットログを更新
    //    Canvas.ForceUpdateCanvases();  // チャットログの更新
    //}
    //private IEnumerator ScrollToBottom()
    //{
    //    yield return new WaitForEndOfFrame();
    //    chatScrollRect.verticalNormalizedPosition = 0; // チャットログを一番下にスクロール
    //}
}
