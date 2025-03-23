using UnityEngine;
using UnityEngine.UI;

public class DotUIManager : MonoBehaviour
{
    public static DotUIManager instance;
    [SerializeField] GameObject dotUI;
    [SerializeField] GameObject blindPanel;
    [SerializeField] Button undoButton;
    [SerializeField] Button redoButton;
    [SerializeField] Button clearButton;
    [SerializeField] Button backButton;
    [SerializeField] GameObject penButtonCover;
    [SerializeField] GameObject fillButtonCover;
    [SerializeField] GameObject lineButtonCover;
    [SerializeField] GameObject circleButtonCover;
    [SerializeField] GameObject rectangleButtonCover;
    [SerializeField] Text roleText;
    [SerializeField] Text themeText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            PhotonManager.instance.OnLeaveRoomAndDestroy();
        });
    }

    private void Update()
    {
        SetActive(dotUI, GameManager.instance.isDrawable());
        SetActive(blindPanel, DrawingManager.instance.isBlind);
        SetActive(penButtonCover, DrawingManager.instance.currentMode == DrawingManager.ToolMode.Pen);
        SetActive(fillButtonCover, DrawingManager.instance.currentMode == DrawingManager.ToolMode.Fill);
        SetActive(lineButtonCover, DrawingManager.instance.currentMode == DrawingManager.ToolMode.Line);
        SetActive(circleButtonCover, DrawingManager.instance.currentMode == DrawingManager.ToolMode.Circle);
        SetActive(rectangleButtonCover, DrawingManager.instance.currentMode == DrawingManager.ToolMode.Rectangle);

        SetInteractable(undoButton, DrawingManager.instance.undoStackCount > 1);
        SetInteractable(redoButton, DrawingManager.instance.redoStackCount > 0);
        SetInteractable(clearButton, DrawingManager.instance.HasDrawing());
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
        string text = role == Role.Questioner ? "あなたは描き手です" : "あなたは回答者です";
        roleText.text = text;
    }

    public void SetThemeText(string answer)
    {
        Role role = GameManager.instance.GetRole();
        if (role == Role.Questioner)
        {
            themeText.text = "お題：" + answer;
        }
        else if (role == Role.Answerer)
        {
            themeText.text = "お題はなんでしょう？";
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
