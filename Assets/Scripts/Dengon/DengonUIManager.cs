using UnityEngine;
using UnityEngine.UI;

public class DengonUIManager : MonoBehaviour
{
    [SerializeField] DengonGridGenerator gridGenerator;

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
        DengonDrawingManager.instance.InitializeDrawField();
    }

    private void Update()
    {
        SetActive(dotUI, DengonDrawingManager.instance.isDrawable);
        SetActive(blindPanel, isBlind);
        SetActive(penButtonCover, DengonDrawingManager.instance.currentMode == DengonDrawingManager.ToolMode.Pen);
        SetActive(fillButtonCover, DengonDrawingManager.instance.currentMode == DengonDrawingManager.ToolMode.Fill);
        SetActive(lineButtonCover, DengonDrawingManager.instance.currentMode == DengonDrawingManager.ToolMode.Line);
        SetActive(circleButtonCover, DengonDrawingManager.instance.currentMode == DengonDrawingManager.ToolMode.Circle);
        SetActive(rectangleButtonCover, DengonDrawingManager.instance.currentMode == DengonDrawingManager.ToolMode.Rectangle);

        SetInteractable(undoButton, DengonDrawingManager.instance.undoStackCount > 1);
        SetInteractable(redoButton, DengonDrawingManager.instance.redoStackCount > 0);
        SetInteractable(clearButton, DengonDrawingManager.instance.HasDrawing());

        currentColor.color = DengonDrawingManager.instance.drawColor;

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

    public void SetThemeText(Role role, string theme)
    {
        themeText.text = role == Role.Questioner ? "お題：" + theme : "お題は何でしょう？";
    }

    public void OnClickSizeApplyButton()
    {
        DengonDrawingManager.instance.ResetDrawFieldSize(widthInputField.inputPixelSize, heightInputField.inputPixelSize);
        if (DengonDrawingManager.instance.CanvasWidth > 50 || DengonDrawingManager.instance.CanvasHeight > 50)
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
        DengonDrawingManager.instance.UndoButton();
    }

    public void OnClickRedoButton()
    {
        DengonDrawingManager.instance.RedoButton();
    }

    public void ToggleIsDrawable()
    {
        DengonDrawingManager.instance.isDrawable = !DengonDrawingManager.instance.isDrawable;
    }

    // ツールボタン
    public void OnClickToolButton(int index)
    {
        switch (index)
        {
            case 0:
                DengonDrawingManager.instance.ChangeMode(DengonDrawingManager.ToolMode.Pen);
                break;
            case 1:
                DengonDrawingManager.instance.ChangeMode(DengonDrawingManager.ToolMode.Fill);
                break;
            case 2:
                DengonDrawingManager.instance.ChangeMode(DengonDrawingManager.ToolMode.Line);
                break;
            case 3:
                DengonDrawingManager.instance.ChangeMode(DengonDrawingManager.ToolMode.Circle);
                break;
            case 4:
                DengonDrawingManager.instance.ChangeMode(DengonDrawingManager.ToolMode.Rectangle);
                break;
        }
    }

    public void OnClickAllClearButton()
    {
        DengonDrawingManager.instance.AllClear();
    }

    public void OnClickBlack()
    {
        DengonDrawingManager.instance.ChangeColor(Color.black);
    }
    public void OnClickRed()
    {
        DengonDrawingManager.instance.ChangeColor(Color.red);
    }
    public void OnClickBlue()
    {
        DengonDrawingManager.instance.ChangeColor(Color.blue);
    }
    public void OnClickGreen()
    {
        DengonDrawingManager.instance.ChangeColor(Color.green);
    }
    public void OnClickYellow()
    {
        DengonDrawingManager.instance.ChangeColor(Color.yellow);
    }
    public void OnClickMagenta()
    {
        DengonDrawingManager.instance.ChangeColor(Color.magenta);
    }
    public void OnClickCyan()
    {
        DengonDrawingManager.instance.ChangeColor(Color.cyan);
    }
    public void OnClickGray()
    {
        DengonDrawingManager.instance.ChangeColor(Color.gray);
    }
    public void OnClickBeige()
    {
        DengonDrawingManager.instance.ChangeColor(new Color32(246, 184, 148, 255));
    }
    public void OnClickWhite()
    {
        DengonDrawingManager.instance.ChangeColor(Color.white);
    }
    public void OnClickEraserButton()
    {
        DengonDrawingManager.instance.ChangeColor(new Color(0, 0, 0, 0));
    }
}
