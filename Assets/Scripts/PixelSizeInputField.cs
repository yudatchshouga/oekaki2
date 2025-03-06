using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class PixelSizeInputField : MonoBehaviour
{
    [SerializeField] InputField inputField;
    public bool isError;

    public int number;

    private void Start()
    {
        inputField.contentType = InputField.ContentType.IntegerNumber;
        inputField.onValueChanged.AddListener(OnInputValueChanged);
        inputField.onEndEdit.AddListener(ValidateInput);
        inputField.text = number.ToString();
    }

    private void ValidateInput(string input)
    {
        // �����ȊO�̓��͂𖳌���
        if (!Regex.IsMatch(input, @"^\d+$"))
        {
            inputField.text = "0";
        }
    }

    private void OnInputValueChanged(string input)
    {
        if (int.TryParse(input, out int value))
        {
            // ���͒l�����������ǂ������`�F�b�N
            if (value >= 1 && value <= 50)
            {
                number = int.Parse(inputField.text);
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

    public void IncrementValue()
    {
        number = int.Parse(inputField.text);
        number++;
        inputField.text = number.ToString();
    }

    public void DecrementValue()
    {
        number = int.Parse(inputField.text);
        number--;
        inputField.text = number.ToString();
    }
}
