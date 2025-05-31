using UnityEngine;
using UnityEngine.UI;

public class DotUIManager : MonoBehaviour
{
    [SerializeField] GridGenerator gridGenerator;

    [SerializeField] GameObject dotUI;
    [SerializeField] GameObject blindPanel;
    [SerializeField] Button undoButton;
    [SerializeField] Button redoButton;
    [SerializeField] Button clearButton;
    [SerializeField] Button sizeApplyButton;
    [SerializeField] Button backButton1;
    [SerializeField] Button backButton2;
    [SerializeField] Button gameRestartButton;
    [SerializeField] GameObject penButtonCover;
    [SerializeField] GameObject fillButtonCover;
    [SerializeField] GameObject lineButtonCover;
    [SerializeField] GameObject circleButtonCover;
    [SerializeField] GameObject rectangleButtonCover;
    [SerializeField] Text roleText;
    [SerializeField] Text themeText;
    [SerializeField] SizeInputField widthInputField;
    [SerializeField] SizeInputField heightInputField;
    [SerializeField] QuestionCountInputField questionCountInputField;
    [SerializeField] LimitTimeInputField limitTimeInputField;
    [SerializeField] Image currentColor;
    [SerializeField] Toggle mekakushiToggle;

    [SerializeField] bool isBlind;

    [SerializeField] GameObject sizeChangerPanel;
    [SerializeField] GameObject colorSpectrum;
    [SerializeField] GameObject backPanel;


    private void Start()
    {
        backButton1.onClick.AddListener(() =>
        {
            PhotonManager.instance.OnLeaveRoomAndDestroy();
        });
        backButton2.onClick.AddListener(() =>
        {
            PhotonManager.instance.OnLeaveRoomAndDestroy();
        });
    }

    public void Initialize()
    {
        // 初期化処理
        gridGenerator.InitializeGridToggle();
        sizeChangerPanel.SetActive(false);
        colorSpectrum.SetActive(false);
        backPanel.SetActive(false);
        mekakushiToggle.isOn = false;
        DrawingManager.instance.InitializeDrawField();
    }

    private void Update()
    {
        SetActive(dotUI, DrawingManager.instance.isDrawable);
        SetActive(blindPanel, isBlind);
        SetActive(penButtonCover, DrawingManager.instance.currentMode == DrawingManager.ToolMode.Pen);
        SetActive(fillButtonCover, DrawingManager.instance.currentMode == DrawingManager.ToolMode.Fill);
        SetActive(lineButtonCover, DrawingManager.instance.currentMode == DrawingManager.ToolMode.Line);
        SetActive(circleButtonCover, DrawingManager.instance.currentMode == DrawingManager.ToolMode.Circle);
        SetActive(rectangleButtonCover, DrawingManager.instance.currentMode == DrawingManager.ToolMode.Rectangle);

        SetInteractable(undoButton, DrawingManager.instance.undoStackCount > 1);
        SetInteractable(redoButton, DrawingManager.instance.redoStackCount > 0);
        SetInteractable(clearButton, DrawingManager.instance.HasDrawing());

        currentColor.color = DrawingManager.instance.drawColor;

        isBlind = mekakushiToggle.isOn;

        if (heightInputField.IsError || widthInputField.IsError)
        {
            SetInteractable(sizeApplyButton, false);
        }
        else
        {
            SetInteractable(sizeApplyButton, true);
        }

        if (questionCountInputField.IsError || limitTimeInputField.IsError)
        {
            SetInteractable(gameRestartButton, false);
        }
        else
        {
            SetInteractable(gameRestartButton, true);
        }
    }

    private void SetActive(GameObject obj, bool isActive)
    {
        if (obj.activeSelf != isActive)
        {
            obj.SetActive(isActive);
        }
    }

    private void SetInteractable(Button button, bool isInteractable)
    {
        if (button.interactable != isInteractable)
        {
            button.interactable = isInteractable;
        }
    }

    public void SetRoleText(Role role)
    {
        roleText.text = role == Role.Questioner ? "あなたは描き手です" : "あなたは回答者です";
    }

    public void SetThemeText(Role role, string theme)
    {
        themeText.text = role == Role.Questioner ? "お題：" + theme : "お題は何でしょう？";
    }

    public void OnClickSizeApplyButton()
    {
        DrawingManager.instance.ResetDrawFieldSize(widthInputField.inputPixelSize, heightInputField.inputPixelSize);
        if (DrawingManager.instance.CanvasWidth > 50 || DrawingManager.instance.CanvasHeight > 50)
        {
            gridGenerator.ChangeInteractableGridToggle(false);
        }
        else
        {
            gridGenerator.ChangeInteractableGridToggle(true);
        }
    }

    public void OnClickUndoButton()
    {
        DrawingManager.instance.UndoButton();
    }

    public void OnClickRedoButton()
    {
        DrawingManager.instance.RedoButton();
    }

    public void ToggleIsDrawable()
    { 
        DrawingManager.instance.isDrawable = !DrawingManager.instance.isDrawable;
    }

    // ツールボタン
    public void OnClickToolButton(int index)
    {
        switch (index)
        {
            case 0:
                DrawingManager.instance.ChangeMode(DrawingManager.ToolMode.Pen);
                break;
            case 1:
                DrawingManager.instance.ChangeMode(DrawingManager.ToolMode.Fill);
                break;
            case 2:
                DrawingManager.instance.ChangeMode(DrawingManager.ToolMode.Line);
                break;
            case 3:
                DrawingManager.instance.ChangeMode(DrawingManager.ToolMode.Circle);
                break;
            case 4:
                DrawingManager.instance.ChangeMode(DrawingManager.ToolMode.Rectangle);
                break;
        }
    }

    public void OnClickAllClearButton()
    {
        DrawingManager.instance.AllClear();
    }

    public void OnClickBlack()
    {
        DrawingManager.instance.ChangeColor(Color.black);
    }
    public void OnClickRed()
    {
        DrawingManager.instance.ChangeColor(Color.red);
    }
    public void OnClickBlue()
    {
        DrawingManager.instance.ChangeColor(Color.blue);
    }
    public void OnClickGreen()
    {
        DrawingManager.instance.ChangeColor(Color.green);
    }
    public void OnClickYellow()
    {
        DrawingManager.instance.ChangeColor(Color.yellow);
    }
    public void OnClickMagenta()
    {
        DrawingManager.instance.ChangeColor(Color.magenta);
    }
    public void OnClickCyan()
    {
        DrawingManager.instance.ChangeColor(Color.cyan);
    }
    public void OnClickGray()
    {
        DrawingManager.instance.ChangeColor(Color.gray);
    }
    public void OnClickBeige()
    {
        DrawingManager.instance.ChangeColor(new Color32(246, 184, 148, 255));
    }
    public void OnClickWhite() 
    {
        DrawingManager.instance.ChangeColor(Color.white);
    }
    public void OnClickEraserButton()
    {
        DrawingManager.instance.ChangeColor(new Color(0, 0, 0, 0));
    }
}
