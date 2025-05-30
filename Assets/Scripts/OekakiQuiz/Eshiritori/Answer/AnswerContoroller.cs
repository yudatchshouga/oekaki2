using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerContoroller : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private EshiritoriManager eshiritoriManager;
    private List<string> answers = new List<string>();

    public void OnSubmit()
    {
        // 入力されたテキストを取得
        string inputText = inputField.text;
        if (string.IsNullOrEmpty(inputText)) return;
        eshiritoriManager.SetAnswer(inputText);
        // 入力フィールドをクリア
        inputField.text = string.Empty;
    }
}
