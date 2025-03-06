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
        if (!widthInputField.isError && !heightInputField.isError)
        {
            errorText.gameObject.SetActive(false);
            startButton.interactable = true;
        }
        else
        {
            errorText.text = "1 ‚©‚ç 50 ‚ÌŠÔ‚Ì®”’l‚ğ“ü—Í‚µ‚Ä‚­‚¾‚³‚¢";
            errorText.gameObject.SetActive(true);
            startButton.interactable = false;
        }
    }
}
