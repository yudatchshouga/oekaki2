using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using System.Collections;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField] InputField chatInputField;
    [SerializeField] ScrollRect chatScrollRect;

    [SerializeField] GameObject textPrefab;
    [SerializeField] Transform chatTransform;
    float r;
    float g;
    float b;
    float a;

    List<string> chatMessages = new List<string>();

    private void Start()
    {
        r = PlayerPrefs.GetFloat("TextColorR", 1f);
        g = PlayerPrefs.GetFloat("TextColorG", 1f);
        b = PlayerPrefs.GetFloat("TextColorB", 1f);
        a = PlayerPrefs.GetFloat("TextColorA", 1f);
    }

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
                photonView.RPC("SendChatMessage", RpcTarget.All, answer, senderName, r, g, b, a);
                chatInputField.text = ""; // チャット入力欄をリセット

                // 出題者以外の場合は回答を提出
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
    private void SendChatMessage(string message, string senderName, float r, float g, float b, float a)
    {
        GameObject newTextObject = Instantiate(textPrefab, chatTransform);
        Text newText = newTextObject.GetComponent<Text>();

        newText.text = $"{senderName}: {message}";
        newText.color = new Color(r, g, b, a); ; // テキストの色を設定

        Canvas.ForceUpdateCanvases();
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return null; // 次のフレームまで待機
        chatScrollRect.verticalNormalizedPosition = 0f; // スクロールを最下部に移動
    }
}
