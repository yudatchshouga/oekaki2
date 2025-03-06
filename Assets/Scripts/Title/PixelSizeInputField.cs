using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class PixelSizeInputField : MonoBehaviour
{
    [SerializeField] InputField inputField;
    public int minValue;
    public int maxValue;
    public int inputPixelSize;
    [SerializeField] bool isError;
    public bool IsError => isError;

    private void Start()
    {
        inputField.contentType = InputField.ContentType.IntegerNumber;
        inputField.onValueChanged.AddListener(OnInputValueChanged);
        inputField.onEndEdit.AddListener(ValidateInput);
        inputField.text = inputPixelSize.ToString();
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
            if (value >= minValue && value <= maxValue)
            {
                inputPixelSize = int.Parse(inputField.text);
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
        inputPixelSize = int.Parse(inputField.text);
        inputPixelSize++;
        inputField.text = inputPixelSize.ToString();
    }

    public void DecrementValue()
    {
        inputPixelSize = int.Parse(inputField.text);
        inputPixelSize--;
        inputField.text = inputPixelSize.ToString();
    }
}
