using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Photon.Pun;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Toggle hostToggle;
    [SerializeField] Toggle guestToggle;
    [SerializeField] Toggle mekakusiToggle;
    [SerializeField] Toggle randomToggle;
    [SerializeField] InputField passwordInputField;
    [SerializeField] InputField limitTimeInputField;
    [SerializeField] GameObject errorText;
    [SerializeField] Button gameStartButton;
    [SerializeField] Button applyButton;
    [SerializeField] Dropdown resolutionDropdown;

    private bool isError;
    [SerializeField] int limitTime;

    private Resolution[] resolutions = {
        new Resolution { width = 640, height = 360 },
        new Resolution { width = 854, height = 480 },
        new Resolution { width = 960, height = 540 },
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 3840, height = 2160 }
    };

    private void Start()
    {
        int savedIndex = PlayerPrefs.GetInt("ResolutionIndex", 5);
        resolutionDropdown.value = savedIndex;
        SetResolution();

        mekakusiToggle.isOn = PlayerPrefs.GetInt("Mekakusi", 0) == 1;
        randomToggle.isOn = PlayerPrefs.GetInt("Random", 0) == 1;

        limitTimeInputField.onValueChanged.AddListener(OnInputValueChanged);
        limitTimeInputField.onEndEdit.AddListener(ValidateInput);
        limitTimeInputField.text = limitTime.ToString();
    }

    private void Update()
    {
        if (isError)
        {
            gameStartButton.interactable = false;
            errorText.SetActive(true);
        }
        else
        {
            gameStartButton.interactable = true;
            errorText.SetActive(false);
        }
    }

    public void OnClickGameStartButton()
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("StartOekakiQuiz", RpcTarget.All);
        }
        else
        {
            SceneController.instance.LoadScene("DotOekaki");
        }
    }

    [PunRPC]
    private void StartOekakiQuiz()
    {
        PlayerPrefs.SetInt("Mekakusi", mekakusiToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Random", randomToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("LimitTime", limitTime);
        SceneController.instance.LoadScene("DotOekaki");
    }

    private void SetResolution()
    {
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, false);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.Save();
    }

    private void ValidateInput(string input)
    {
        // 数字以外の入力を無効化
        if (!Regex.IsMatch(input, @"^\d+$"))
        {
            limitTimeInputField.text = "0";
        }
    }

    private void OnInputValueChanged(string input)
    {
        if (int.TryParse(input, out int value))
        {
            // 入力値が制限内かどうかをチェック
            if (value >= 1 && value <= 999)
            {
                limitTime = int.Parse(limitTimeInputField.text);
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

