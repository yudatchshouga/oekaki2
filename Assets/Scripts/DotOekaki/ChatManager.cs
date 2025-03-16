using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
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
            SubmitChatMessage();
        }
    }

    public void SubmitChatMessage()
    {
        string message = chatInputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            chatMessages.Add(message);
            chatInputField.text = ""; // チャット入力欄をリセット
            UpdateChatLog();
            StartCoroutine(ScrollToBottom());
        }
    }

    private void UpdateChatLog()
    {
        chatLogText.text = string.Join("\n", chatMessages.ToArray());　// チャットログを更新
        Canvas.ForceUpdateCanvases();  // チャットログの更新
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        chatScrollRect.verticalNormalizedPosition = 0; // チャットログを一番下にスクロール
    }
}
