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
                chatMessages.Add(answer);
                // チャット入力欄をリセット
                chatInputField.text = "";
                chatLogText.text = string.Join("\n", chatMessages.ToArray());
                Canvas.ForceUpdateCanvases(); // ワンフレーム待つ必要あり？
                chatScrollRect.verticalNormalizedPosition = 0;
            }
            // チャット入力欄にフォーカスを移す
            chatInputField.Select();
            chatInputField.ActivateInputField();

            // RPCで送信
            photonView.RPC("SendChatMessage", RpcTarget.Others, answer);
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
    public void SendChatMessage(string message)
    {
        chatMessages.Add(message);
        chatLogText.text = string.Join("\n", chatMessages.ToArray());
        Canvas.ForceUpdateCanvases();
        // ワンフレーム待つ必要あり？
        chatScrollRect.verticalNormalizedPosition = 0;

        // 正誤判定
        themeGenerator.CheckAnswer(message);
    }
}
