using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    public InputField chatInputField;
    public Text chatLogText;
    public ScrollRect chatScrollRect;

    private List<string> chatMessages = new List<string>();

    void Update()
    {
        // �G���^�[�L�[�܂��̓e���L�[�̃G���^�[�L�[�������ꂽ�烁�b�Z�[�W�𑗐M
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
            chatInputField.text = "";
            UpdateChatLog();
        }
    }

    private void UpdateChatLog()
    {
        chatLogText.text = string.Join("\n", chatMessages.ToArray());
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0;
    }
}
