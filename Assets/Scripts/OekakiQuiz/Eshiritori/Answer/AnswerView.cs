using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnsweView : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    public System.Action<string> OnSubmitAnswer;

    public void OnSubmit()
    {
        // 入力されたテキストを取得
        string inputText = inputField.text;
        if (string.IsNullOrEmpty(inputText)) return;
        OnSubmitAnswer?.Invoke(inputText);
        // 入力フィールドをクリア
        inputField.text = string.Empty;
    }
}
