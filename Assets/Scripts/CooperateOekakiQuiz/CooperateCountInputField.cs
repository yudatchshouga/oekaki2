using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class CooperateCountInputField : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] bool isError;
    public bool IsError => isError;

    private void Start()
    {
        inputField.contentType = InputField.ContentType.IntegerNumber;
        inputField.onValueChanged.AddListener(OnInputValueChanged);
        inputField.onEndEdit.AddListener(ValidateInput);
        inputField.text = 5.ToString(); // 初期値を5に設定
    }

    private void ValidateInput(string input)
    {
        // 数字以外の入力を無効化
        if (!Regex.IsMatch(input, @"^\d+$"))
        {
            inputField.text = "0";
        }
    }

    private void OnInputValueChanged(string input)
    {
        if (int.TryParse(input, out int value))
        {
            // 入力値が制限内かどうかをチェック
            if (value >= 1 && value <= 10)
            {
                PlayerPrefs.SetInt("CooperateCount", value);
                isError = false;
            }
            else
            {
                isError = true;
            }
        }
        else
        {
            isError = true;
        }
    }
}
