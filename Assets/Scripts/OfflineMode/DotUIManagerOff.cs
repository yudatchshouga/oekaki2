using UnityEngine;
using UnityEngine.UI;

public class DotUIManagerOff : MonoBehaviour
{
    [SerializeField] GridGeneratorOff gridGeneratorOff;

    [SerializeField] GameObject dotUI;
    [SerializeField] GameObject blindPanel;
    [SerializeField] Image currentColor;
    [SerializeField] Button undoButton;
    [SerializeField] Button redoButton;
    [SerializeField] Button clearButton;
    [SerializeField] Button sizeApplyButton;
    [SerializeField] Button backButton;
    [SerializeField] GameObject penButtonCover;
    [SerializeField] GameObject fillButtonCover;
    [SerializeField] GameObject lineButtonCover;
    [SerializeField] GameObject circleButtonCover;
    [SerializeField] GameObject rectangleButtonCover;
    [SerializeField] SizeInputField widthInputField;
    [SerializeField] SizeInputField heightInputField;

    private void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            SceneController.instance.LoadScene("Title");
        });
    }

    private void Update()
    {
        SetActive(dotUI, DrawingManagerOff.instance.isDrawable);
        SetActive(blindPanel, DrawingManagerOff.instance.isBlind);
        SetActive(penButtonCover, DrawingManagerOff.instance.currentMode == DrawingManagerOff.ToolMode.Pen);
        SetActive(fillButtonCover, DrawingManagerOff.instance.currentMode == DrawingManagerOff.ToolMode.Fill);
        SetActive(lineButtonCover, DrawingManagerOff.instance.currentMode == DrawingManagerOff.ToolMode.Line);
        SetActive(circleButtonCover, DrawingManagerOff.instance.currentMode == DrawingManagerOff.ToolMode.Circle);
        SetActive(rectangleButtonCover, DrawingManagerOff.instance.currentMode == DrawingManagerOff.ToolMode.Rectangle);

        SetInteractable(undoButton, DrawingManagerOff.instance.undoStackCount > 1);
        SetInteractable(redoButton, DrawingManagerOff.instance.redoStackCount > 0);
        SetInteractable(clearButton, DrawingManagerOff.instance.HasDrawing());

        currentColor.color = DrawingManagerOff.instance.drawColor;

        if (heightInputField.IsError || widthInputField.IsError)
        {
            SetInteractable(sizeApplyButton, false);
        }
        else
        {
            SetInteractable(sizeApplyButton, true);
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

    public void OnClickSizeApplyButton()
    {
        DrawingManagerOff.instance.ResetDrawFieldSize(widthInputField.inputPixelSize, heightInputField.inputPixelSize);
        if (DrawingManagerOff.instance.CanvasWidth > 50 || DrawingManagerOff.instance.CanvasHeight > 50)
        {
            gridGeneratorOff.ChangeInteractableGridToggle(false);
        }
        else
        {
            gridGeneratorOff.ChangeInteractableGridToggle(true);
        }
    }

    public void OnClickUndoButton()
    {
        DrawingManagerOff.instance.UndoButton();
    }

    public void OnClickRedoButton()
    {
        DrawingManagerOff.instance.RedoButton();
    }

    public void ToggleIsDrawable()
    {
        DrawingManagerOff.instance.isDrawable = !DrawingManagerOff.instance.isDrawable;
    }

    // ツールボタン
    public void OnClickToolButton(int index)
    {
        switch (index)
        {
            case 0:
                DrawingManagerOff.instance.ChangeMode(DrawingManagerOff.ToolMode.Pen);
                break;
            case 1:
                DrawingManagerOff.instance.ChangeMode(DrawingManagerOff.ToolMode.Fill);
                break;
            case 2:
                DrawingManagerOff.instance.ChangeMode(DrawingManagerOff.ToolMode.Line);
                break;
            case 3:
                DrawingManagerOff.instance.ChangeMode(DrawingManagerOff.ToolMode.Circle);
                break;
            case 4:
                DrawingManagerOff.instance.ChangeMode(DrawingManagerOff.ToolMode.Rectangle);
                break;
        }
    }

    public void OnClickAllClearButton()
    {
        DrawingManagerOff.instance.AllClear();
    }

    public void OnClickBlack()
    {
        DrawingManagerOff.instance.ChangeColor(Color.black);
    }
    public void OnClickRed()
    {
        DrawingManagerOff.instance.ChangeColor(Color.red);
    }
    public void OnClickBlue()
    {
        DrawingManagerOff.instance.ChangeColor(Color.blue);
    }
    public void OnClickGreen()
    {
        DrawingManagerOff.instance.ChangeColor(Color.green);
    }
    public void OnClickYellow()
    {
        DrawingManagerOff.instance.ChangeColor(Color.yellow);
    }
    public void OnClickMagenta()
    {
        DrawingManagerOff.instance.ChangeColor(Color.magenta);
    }
    public void OnClickCyan()
    {
        DrawingManagerOff.instance.ChangeColor(Color.cyan);
    }
    public void OnClickGray()
    {
        DrawingManagerOff.instance.ChangeColor(Color.gray);
    }
    public void OnClickBeige()
    {
        DrawingManagerOff.instance.ChangeColor(new Color32(246, 184, 148, 255));
    }
    public void OnClickWhite()
    {
        DrawingManagerOff.instance.ChangeColor(Color.white);
    }
    public void OnClickEraserButton()
    {
        DrawingManagerOff.instance.ChangeColor(new Color(0, 0, 0, 0));
    }
}
