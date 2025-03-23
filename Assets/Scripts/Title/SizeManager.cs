using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SizeManager : MonoBehaviourPunCallbacks
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
            errorText.text = $"{widthInputField.minValue} から {widthInputField.maxValue} の間の整数値を入力してください";
            errorText.gameObject.SetActive(true);
            startButton.interactable = false;
        }
    }

    public void OnStartButtonClick()
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("SetWidthAndHeight", RpcTarget.All, widthInputField.inputPixelSize, heightInputField.inputPixelSize);
            photonView.RPC("StartOekakiQuiz", RpcTarget.All);
        }
        else
        {
            SetWidthAndHeight(widthInputField.inputPixelSize, heightInputField.inputPixelSize);
            StartOekakiQuiz();
        }
    }

    [PunRPC]
    private void StartOekakiQuiz()
    {
        SceneController.instance.LoadScene("DotOekaki");
    }

    [PunRPC]
    private void SetWidthAndHeight(int width, int height)
    {
        PlayerPrefs.SetInt("Width", width);
        PlayerPrefs.SetInt("Height", height);
    }
}
