using UnityEngine;
using UnityEngine.UI;

public class AnswerContoroller : MonoBehaviour
{
    [SerializeField] private InputField inputField; // 入力フィールド
    // ボタンを押すと、inputFieldのテキストを取得する
    public void OnSubmit()
    {
        // 入力されたテキストを取得
        string inputText = inputField.text;
        if (string.IsNullOrEmpty(inputText)) return;
        Debug.Log("入力されたテキスト: " + inputText);
        // 入力フィールドをクリア
        inputField.text = string.Empty;
    }
}
