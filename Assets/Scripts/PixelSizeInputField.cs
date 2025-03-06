using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class PixelSizeInputField : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] Text errorText;
    [SerializeField] Button startButton;

    public int number;

    private void Start()
    {
        inputField.contentType = InputField.ContentType.IntegerNumber;
        inputField.onValueChanged.AddListener(OnInputValueChanged);
        inputField.onEndEdit.AddListener(ValidateInput);
        inputField.text = number.ToString();

        startButton.interactable = true;
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
                errorText.gameObject.SetActive(false);
                number = int.Parse(inputField.text);
                startButton.interactable = true;
            }
            else
            {
                errorText.text = "1 ���� 50 �̊Ԃ̐����l����͂��Ă�������";
                errorText.gameObject.SetActive(true);
                startButton.interactable = false;
            }
        }
        else
        {
            errorText.text = "1 ���� 50 �̊Ԃ̐����l����͂��Ă�������";
            errorText.gameObject.SetActive(true);
            startButton.interactable = false;
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
