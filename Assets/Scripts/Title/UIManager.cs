using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] InputField createPasswordInputField;
    [SerializeField] InputField joinPasswordInputField;
    [SerializeField] Dropdown playerCountDropdown;
    [SerializeField] Text createErrorText;
    [SerializeField] Text joinErrorText;
    [SerializeField] Button roomCreateButton;
    [SerializeField] Button roomJoinButton;
    [SerializeField] Toggle tsuyuToggle;
    [SerializeField] int playerCount;

    // おえかきクイズモード
    [SerializeField] Button quizGameStartButton;
    [SerializeField] InputField questionCountInputField;
    [SerializeField] InputField limitTimeInputField;
    [SerializeField] int questionCount;
    [SerializeField] int limitTime;
    private bool isErrorQuestionCount;
    private bool isErrorLimitTime;

    // 協力クイズモード
    [SerializeField] Button cooperateQuizButton;
    [SerializeField] Button cooperateQuizStartButton;
    [SerializeField] InputField cooperateCountInputField;
    [SerializeField] InputField cooperateTimeInputField;
    [SerializeField] int cooperateCount;
    [SerializeField] int cooperateTime;
    private bool isErrorCooperateCount;
    private bool isErrorCooperateTime;

    // 絵しりとりモード
    [SerializeField] Button shiritoriStartButton;
    [SerializeField] InputField shiritoriTimeInputField;
    [SerializeField] InputField shiritoriAnswerTimeInputField;
    [SerializeField] int shiritoriTime;
    [SerializeField] int shiritoriAnswerTime;
    private bool isErrorShiritoriTime;
    private bool isErrorShiritoriAnswerTime;

    // 伝言ゲームモード
    [SerializeField] Button dengonStartButton;
    [SerializeField] InputField dengonTimeInputField;
    [SerializeField] InputField dengonAnswerTimeInputField;
    [SerializeField] int dengonTime;
    [SerializeField] int dengonAnswerTime;
    private bool isErrorDengonTime;
    private bool isErrorDengonAnswerTime;

    // オプションメニュー
    [SerializeField] InputField playerNameInputField;
    [SerializeField] Image textColorImage;
    [SerializeField] Text textColorText;
    [SerializeField] Dropdown resolutionDropdown;
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
        int savedIndex = PlayerPrefs.GetInt("ResolutionIndex", 2);
        resolutionDropdown.value = savedIndex;
        SetResolution();

        questionCountInputField.onValueChanged.AddListener(OnQuestionCountInputValueChanged);
        questionCountInputField.onEndEdit.AddListener(ValidateQuestionCountInput);
        questionCountInputField.text = questionCount.ToString();
        limitTimeInputField.onValueChanged.AddListener(OnLimitTextInputValueChanged);
        limitTimeInputField.onEndEdit.AddListener(ValidateLimitTextInput);
        limitTimeInputField.text = limitTime.ToString();

        cooperateCountInputField.onValueChanged.AddListener(OnCooperateCountInputValueChanged);
        cooperateCountInputField.onEndEdit.AddListener(ValidateCooperateCountInput);
        cooperateCountInputField.text = cooperateCount.ToString();
        cooperateTimeInputField.onValueChanged.AddListener(OnCooperateTimeInputValueChanged);
        cooperateTimeInputField.onEndEdit.AddListener(ValidateCooperateTimeInput);
        cooperateTimeInputField.text = cooperateTime.ToString();

        shiritoriTimeInputField.onValueChanged.AddListener(OnShiritoriTimeInputValueChanged);
        shiritoriTimeInputField.onEndEdit.AddListener(ValidateShiritoriTimeInput);
        shiritoriTimeInputField.text = shiritoriTime.ToString();
        shiritoriAnswerTimeInputField.onValueChanged.AddListener(OnShiritoriAnswerTimeInputValueChanged);
        shiritoriAnswerTimeInputField.onEndEdit.AddListener(ValidateShiritoriAnswerTimeInput);
        shiritoriAnswerTimeInputField.text = shiritoriAnswerTime.ToString();

        dengonTimeInputField.onValueChanged.AddListener(OnDengonTimeInputValueChanged);
        dengonTimeInputField.onEndEdit.AddListener(ValidateDengonTimeInput);
        dengonTimeInputField.text = dengonTime.ToString();
        dengonAnswerTimeInputField.onValueChanged.AddListener(OnDengonAnswerTimeInputValueChanged);
        dengonAnswerTimeInputField.onEndEdit.AddListener(ValidateDengonAnswerTimeInput);
        dengonAnswerTimeInputField.text = dengonAnswerTime.ToString();

        createPasswordInputField.onValueChanged.AddListener(OnCreatePasswordInputFieldValueChanged);
        joinPasswordInputField.onValueChanged.AddListener(OnJoinPasswordInputFieldValueChanged);

        playerCountDropdown.onValueChanged.AddListener(OnPlayerCountDropdownValueChanged);
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (!isErrorQuestionCount && !isErrorLimitTime)
            {
                quizGameStartButton.interactable = true;
            }
            else
            {
                quizGameStartButton.interactable = false;
            }

            if (!isErrorCooperateCount && !isErrorCooperateTime)
            {
                cooperateQuizStartButton.interactable = true;
            }
            else
            {
                cooperateQuizStartButton.interactable = false;
            }

            if (!isErrorDengonTime && !isErrorDengonAnswerTime)
            {
                dengonStartButton.interactable = true;
            }
            else
            {
                dengonStartButton.interactable = false;
            }

            if (!isErrorShiritoriTime && !isErrorShiritoriAnswerTime)
            {
                shiritoriStartButton.interactable = true;
            }
            else
            {
                shiritoriStartButton.interactable = false;
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount >= 3)
            {
                cooperateQuizButton.interactable = true;
            }
            else
            {
                cooperateQuizButton.interactable = false;
            }
        }
    }

    // --------------- ボタン ---------------

    public void OnCreateRoomButtonClick()
    {
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)playerCount;
        roomOptions.IsVisible = false;

        PhotonNetwork.CreateRoom(createPasswordInputField.text, roomOptions, TypedLobby.Default);
    }

    // ルーム作成成功時のコールバック
    public override void OnCreatedRoom()
    {
        PanelController.instance.OnClickButton(4);
    }

    // ルーム作成失敗時のコールバック
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if (returnCode == ErrorCode.InvalidOperation)
        {
            Debug.LogError("ルームの作成に失敗しました: " + message);
            ShowCreateErrorMessage("ルームの作成に失敗しました。パスワードが既に使用されています。");
        }
        else
        {
            Debug.LogError("ルームの作成に失敗しました: " + message);
            ShowCreateErrorMessage("ルームの作成に失敗しました。");
        }
    }

    private void ShowCreateErrorMessage(string message)
    {
        createErrorText.gameObject.SetActive(true);
        createErrorText.text = message;
    }

    public void OnJoinRoomButtonClick()
    {
        PhotonNetwork.JoinRoom(joinPasswordInputField.text);
    }

    // ルーム参加成功時のコールバック
    public override void OnJoinedRoom()
    {
        PanelController.instance.OnClickButton(4);
    }

    // ルーム参加失敗時のコールバック
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (returnCode == ErrorCode.GameDoesNotExist)
        {
            Debug.LogError("ルームが存在しません");
            ShowJoinErrorMessage("ルームが存在しません。ルーム名を確認してください。");
        }
        else if (returnCode == ErrorCode.GameFull)
        {
            Debug.LogError("ルームが満員です。");
            ShowJoinErrorMessage("ルームが満員です。再度パスワードを入力してください。");
        }
        else if (returnCode == ErrorCode.GameClosed)
        {
            Debug.LogError("ルームはすでに終了しています");
            ShowJoinErrorMessage("ルームはすでに終了しています。");
        }
        else
        {
            Debug.LogError($"ルームへの参加に失敗しました: {message}");
            ShowJoinErrorMessage($"ルームへの参加に失敗しました: {message}");
        }
    }

    private void ShowJoinErrorMessage(string message)
    {
        joinErrorText.gameObject.SetActive(true);
        joinErrorText.text = message;
    }

    // エラーメッセージを非表示にし、インプットフィールドを初期化
    public void OnClickErrorMessageCloseButton()
    {
        createErrorText.gameObject.SetActive(false);
        joinErrorText.gameObject.SetActive(false);
        createPasswordInputField.text = "";
        joinPasswordInputField.text = "";
    }


    public void OnClickOfflineStartButton()
    {
        SceneController.instance.LoadScene("Offline");
    }

    public void OnClickOekakiQuizStartButton()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            PlayerPrefs.SetInt("QuestionCount", questionCount);
            PlayerPrefs.SetInt("LimitTime", limitTime);
            PlayerPrefs.SetInt("Tsuyu", tsuyuToggle.isOn ? 1 : 0);
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
        SceneController.instance.LoadScene("OekakiQuiz");
    }

    public void OnClickCooperateQuizStartButton()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            PlayerPrefs.SetInt("CooperateCount", cooperateCount);
            PlayerPrefs.SetInt("CooperateTime", cooperateTime);
            photonView.RPC("StartCooperateQuiz", RpcTarget.All);
        }
        else
        {
            SceneController.instance.LoadScene("CooperateQuiz");
        }
    }

    [PunRPC]
    private void StartCooperateQuiz()
    {
        SceneController.instance.LoadScene("CooperateQuiz");
    }

    public void OnClickShiritoriStartButton()
    {
        if (PhotonNetwork.InRoom)
        {
            PlayerPrefs.SetInt("ShiritoriTime", shiritoriTime);
            PlayerPrefs.SetInt("ShiritoriAnswerTime", shiritoriAnswerTime);
            photonView.RPC("StartShiritori", RpcTarget.All);
        }
        else
        {
            SceneController.instance.LoadScene("Eshiritori");
        }
    }

    [PunRPC]
    private void StartShiritori()
    {
        SceneController.instance.LoadScene("Eshiritori");
    }

    public void OnClickDengonButton()
    {
        if (PhotonNetwork.InRoom)
        {
            PlayerPrefs.SetInt("DengonTime", dengonTime);
            PlayerPrefs.SetInt("DengonAnswerTime", dengonAnswerTime);
            photonView.RPC("StartDengon", RpcTarget.All);
        }
        else
        {
            SceneController.instance.LoadScene("Dengon");
        }
    }

    [PunRPC]
    private void StartDengon()
    {
        SceneController.instance.LoadScene("Dengon");
    }


    // --------------- Dropdown ---------------
    public void OnPlayerCountDropdownValueChanged(int value)
    {
        playerCount = playerCountDropdown.value + 2;
    }


    // --------------- InputField ---------------

    // ルーム作成、参加時の処理
    private void OnCreatePasswordInputFieldValueChanged(string input)
    { 
        roomCreateButton.interactable = !string.IsNullOrEmpty(createPasswordInputField.text);
    }

    private void OnJoinPasswordInputFieldValueChanged(string input)
    {
        roomJoinButton.interactable = !string.IsNullOrEmpty(joinPasswordInputField.text);
    }


    // おえかきクイズモード
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
            if (value >= 30 && value <= 999)
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


    // 協力クイズモード
    private void ValidateCooperateCountInput(string input)
    {
        // 数字以外の入力を無効化
        if (!Regex.IsMatch(input, @"^\d+$"))
        {
            cooperateCountInputField.text = "0";
        }
    }
    private void ValidateCooperateTimeInput(string input)
    {
        // 数字以外の入力を無効化
        if (!Regex.IsMatch(input, @"^\d+$"))
        {
            cooperateTimeInputField.text = "0";
        }
    }
    private void OnCooperateCountInputValueChanged(string input)
    {
        if (int.TryParse(input, out int value))
        {
            cooperateCount = int.Parse(cooperateCountInputField.text);
            // 入力値が制限内かどうかをチェック
            if (value >= 1 && value <= 10)
            {
                isErrorCooperateCount = false;
            }
            else
            {
                isErrorCooperateCount = true;
            }
        }
        else
        {
            isErrorCooperateCount = true;
        }
    }
    private void OnCooperateTimeInputValueChanged(string input)
    {
        if (int.TryParse(input, out int value))
        {
            cooperateTime = int.Parse(cooperateTimeInputField.text);
            // 入力値が制限内かどうかをチェック
            if (value >= 30 && value <= 999)
            {
                isErrorCooperateTime = false;
            }
            else
            {
                isErrorCooperateTime = true;
            }
        }
        else
        {
            isErrorCooperateTime = true;
        }
    }


    // 絵しりとりモード
    private void ValidateShiritoriTimeInput(string input)
    {
        // 数字以外の入力を無効化
        if (!Regex.IsMatch(input, @"^\d+$"))
        {
            shiritoriTimeInputField.text = "0";
        }
    }
    private void ValidateShiritoriAnswerTimeInput(string input)
    {
        // 数字以外の入力を無効化
        if (!Regex.IsMatch(input, @"^\d+$"))
        {
            shiritoriAnswerTimeInputField.text = "0";
        }
    }
    private void OnShiritoriTimeInputValueChanged(string input)
    {
        if (int.TryParse(input, out int value))
        {
            shiritoriTime = int.Parse(shiritoriTimeInputField.text);
            // 入力値が制限内かどうかをチェック
            if (value >= 10 && value <= 999)
            {
                isErrorShiritoriTime = false;
            }
            else
            {
                isErrorShiritoriTime = true;
            }
        }
        else
        {
            isErrorShiritoriTime = true;
        }
    }
    private void OnShiritoriAnswerTimeInputValueChanged(string input)
    {
        if (int.TryParse(input, out int value))
        {
            shiritoriAnswerTime = int.Parse(shiritoriAnswerTimeInputField.text);
            // 入力値が制限内かどうかをチェック
            if (value >= 10 && value <= 300)
            {
                isErrorShiritoriAnswerTime = false;
            }
            else
            {
                isErrorShiritoriAnswerTime = true;
            }
        }
        else
        {
            isErrorShiritoriAnswerTime = true;
        }
    }


    // 伝言ゲームモード
    private void ValidateDengonTimeInput(string input)
    {
        // 数字以外の入力を無効化
        if (!Regex.IsMatch(input, @"^\d+$"))
        {
            dengonTimeInputField.text = "0";
        }
    }
    private void ValidateDengonAnswerTimeInput(string input)
    {
        // 数字以外の入力を無効化
        if (!Regex.IsMatch(input, @"^\d+$"))
        {
            dengonAnswerTimeInputField.text = "0";
        }
    }
    private void OnDengonTimeInputValueChanged(string input)
    {
        if (int.TryParse(input, out int value))
        {
            dengonTime = int.Parse(dengonTimeInputField.text);
            // 入力値が制限内かどうかをチェック
            if (value >= 10 && value <= 999)
            {
                isErrorDengonTime = false;
            }
            else
            {
                isErrorDengonTime = true;
            }
        }
        else
        {
            isErrorDengonTime = true;
        }
    }
    private void OnDengonAnswerTimeInputValueChanged(string input)
    {
        if (int.TryParse(input, out int value))
        {
            dengonAnswerTime = int.Parse(dengonAnswerTimeInputField.text);
            // 入力値が制限内かどうかをチェック
            if (value >= 10 && value <= 300)
            {
                isErrorDengonAnswerTime = false;
            }
            else
            {
                isErrorDengonAnswerTime = true;
            }
        }
        else
        {
            isErrorDengonAnswerTime = true;
        }
    }

    // --------------- オプションメニュー ---------------

    // 決定ボタン押下時にプレイヤー名を保存
    public void OnClickOptionApplyButton()
    {
        string playerName = string.IsNullOrEmpty(playerNameInputField.text) ? $"Player{Random.Range(1000, 9999)}" : playerNameInputField.text;
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
    }

    // 画面遷移時にプレイヤー名を取得・表示
    public void DisplayPlayerName()
    {
        playerNameInputField.text = PlayerPrefs.GetString("PlayerName");
        float r = PlayerPrefs.GetFloat("TextColorR", 1f);
        float g = PlayerPrefs.GetFloat("TextColorG", 1f);
        float b = PlayerPrefs.GetFloat("TextColorB", 1f);
        float a = PlayerPrefs.GetFloat("TextColorA", 1f);
        textColorText.color = new Color(r, g, b, a);
        textColorImage.color = new Color(r, g, b, a);
    }

    // デバッグ用
    public void OnClickClearButton()
    {
        playerNameInputField.text = "";
        PlayerPrefs.SetString("PlayerName", "");
        PlayerPrefs.Save();
    }

    public void OnTextColorButtonClick(int index)
    {
        switch (index)
        {
            case 0:
                textColorText.color = Color.red;
                textColorImage.color = Color.red;
                break;
            case 1:
                textColorText.color = Color.blue;
                textColorImage.color = Color.blue;
                break;
            case 2:
                textColorText.color = Color.green;
                textColorImage.color = Color.green;
                break;
            case 3:
                textColorText.color = Color.yellow;
                textColorImage.color = Color.yellow;
                break;
            case 4:
                textColorText.color = Color.cyan;
                textColorImage.color = Color.cyan;
                break;
            case 5:
                textColorText.color = Color.magenta;
                textColorImage.color = Color.magenta;
                break;
            case 6:
                textColorText.color = new Color32(67, 0, 255, 255);
                textColorImage.color = new Color32(67, 0, 255, 255);
                break;
            case 7:
                textColorText.color = new Color32(254, 93, 38, 255);
                textColorImage.color = new Color32(254, 93, 38, 255);
                break;
            case 8:
                textColorText.color = new Color32(16, 46, 80, 255);
                textColorImage.color = new Color32(16, 46, 80, 255);
                break;
            case 9:
                textColorText.color = new Color32(242, 226, 177, 255);
                textColorImage.color = new Color32(242, 226, 177, 255);
                break;
            case 10:
                textColorText.color = new Color32(143, 135, 241, 255);
                textColorImage.color = new Color32(143, 135, 241, 255);
                break;
            case 11:
                textColorText.color = new Color32(148, 80, 52, 255);
                textColorImage.color = new Color32(148, 80, 52, 255);
                break;
            default:
                textColorText.color = Color.white;
                textColorImage.color = Color.white;
                break;
        }
        PlayerPrefs.SetFloat("TextColorR", textColorText.color.r);
        PlayerPrefs.SetFloat("TextColorG", textColorText.color.g);
        PlayerPrefs.SetFloat("TextColorB", textColorText.color.b);
        PlayerPrefs.SetFloat("TextColorA", textColorText.color.a);
        PlayerPrefs.Save();
    }

    private void SetResolution()
    {
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, false);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.Save();
    }
}

