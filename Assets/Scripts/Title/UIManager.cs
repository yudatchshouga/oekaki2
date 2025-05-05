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
    [SerializeField] InputField playerNameInputField;
    [SerializeField] InputField passwordInputField;
    [SerializeField] InputField questionCountInputField;
    [SerializeField] InputField limitTimeInputField;
    [SerializeField] Button gameStartButton;
    [SerializeField] Button applyButton;
    [SerializeField] Dropdown resolutionDropdown;

    private bool isErrorQuestionCount;
    private bool isErrorLimitTime;
    private bool isErrorQuestionner;
    [SerializeField] int questionCount;
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

        questionCountInputField.onValueChanged.AddListener(OnQuestionCountInputValueChanged);
        questionCountInputField.onEndEdit.AddListener(ValidateQuestionCountInput);
        questionCountInputField.text = questionCount.ToString();

        limitTimeInputField.onValueChanged.AddListener(OnLimitTextInputValueChanged);
        limitTimeInputField.onEndEdit.AddListener(ValidateLimitTextInput);
        limitTimeInputField.text = limitTime.ToString();

        int count = FindObjectsByType<PhotonManager>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
        Debug.Log("PhotonManagerの数: " + count);
    }

    private void Update()
    {
        if (!isErrorQuestionCount && !isErrorLimitTime && !isErrorQuestionner)
        {
            gameStartButton.interactable = true;
        }
        else
        {
            gameStartButton.interactable = false;
        }
    }

    public void OnClickOfflineStartButton()
    {
        SceneController.instance.LoadScene("Offline");
    }

    public void OnClickOekakiQuizStartButton()
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("StartOekakiQuiz", RpcTarget.All);
        }
        else
        {
            SceneController.instance.LoadScene("OekakiQuiz");
        }
    }

    [PunRPC]
    private void StartOekakiQuiz()
    {
        PlayerPrefs.SetInt("Mekakusi", mekakusiToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Random", randomToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("QuestionCount", questionCount);
        PlayerPrefs.SetInt("LimitTime", limitTime);
        SceneController.instance.LoadScene("OekakiQuiz");
    }

    // --------------- オプション ---------------

    // 決定ボタン押下時にプレイヤー名を保存
    public void OnClickOptionApplyButton()
    {
        string playerName = string.IsNullOrEmpty(playerNameInputField.text) ? "名無しさん" : playerNameInputField.text;
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
    }

    // 画面遷移時にプレイヤー名を取得・表示
    public void DisplayPlayerName()
    {
        Debug.Log("保存された値: " + PlayerPrefs.GetString("PlayerName"));
        playerNameInputField.text = PlayerPrefs.GetString("PlayerName");
    }

    // デバッグ用
    public void OnClickClearButton()
    {
        playerNameInputField.text = "";
        PlayerPrefs.SetString("PlayerName", "");
        PlayerPrefs.Save();
    }

    private void SetResolution()
    {
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, false);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.Save();
    }

    // --------------- InputField設定 ---------------

    private void ValidateQuestionCountInput(string input)
    {
        // 数字以外の入力を無効化
        if (!Regex.IsMatch(input, @"^\d+$"))
        {
            questionCountInputField.text = "0";
        }
    }
    private void ValidateLimitTextInput(string input)
    {
        // 数字以外の入力を無効化
        if (!Regex.IsMatch(input, @"^\d+$"))
        {
            limitTimeInputField.text = "0";
        }
    }

    private void OnQuestionCountInputValueChanged(string input)
    {
        if (int.TryParse(input, out int value))
        {
            questionCount = int.Parse(questionCountInputField.text);
            // 入力値が制限内かどうかをチェック
            if (value >= 1 && value <= 10)
            {
                isErrorQuestionCount = false;
            }
            else
            {
                isErrorQuestionCount = true;
            }
        }
        else
        {
            isErrorQuestionCount = true;
        }
    }
    private void OnLimitTextInputValueChanged(string input)
    {
        if (int.TryParse(input, out int value))
        {
            limitTime = int.Parse(limitTimeInputField.text);
            // 入力値が制限内かどうかをチェック
            if (value >= 1 && value <= 999)
            {
                isErrorLimitTime = false;
            }
            else
            {
                isErrorLimitTime = true;
            }
        }
        else
        {
            isErrorLimitTime = true;
        }
    }
}

