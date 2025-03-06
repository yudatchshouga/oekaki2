using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    [SerializeField] PixelSizeInputField widthInputField;
    [SerializeField] PixelSizeInputField heightInputField;

    public void OnStartButtonClick()
    {
        PlayerPrefs.SetInt("Width", widthInputField.inputPixelSize);
        PlayerPrefs.SetInt("Height", heightInputField.inputPixelSize);
        PlayerPrefs.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("DotOekaki");
    }

    public void OnBackButtonClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene1");
    }
}
