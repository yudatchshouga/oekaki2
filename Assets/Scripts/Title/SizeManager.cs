using UnityEngine;
using UnityEngine.UI;

public class SizeManager : MonoBehaviour
{
    [SerializeField] PixelSizeInputField widthInputField;
    [SerializeField] PixelSizeInputField heightInputField;
    [SerializeField] Text errorText;
    [SerializeField] Button startButton;

    private void Update()
    {
        if (!widthInputField.IsError && !heightInputField.IsError)
        {
            errorText.gameObject.SetActive(false);
            startButton.interactable = true;
        }
        else
        {
            errorText.text = $"{widthInputField.minValue} ‚©‚ç {widthInputField.maxValue} ‚ÌŠÔ‚Ì®”’l‚ğ“ü—Í‚µ‚Ä‚­‚¾‚³‚¢";
            errorText.gameObject.SetActive(true);
            startButton.interactable = false;
        }
    }
}
