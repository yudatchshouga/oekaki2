using UnityEngine;
using UnityEngine.UI;

public class DotButtonController : MonoBehaviour
{
    [SerializeField] GameObject dotUI;
    [SerializeField] Button undoButton;
    [SerializeField] Button redoButton;
    [SerializeField] Button clearButton;
    [SerializeField] Button backButton;
    [SerializeField] GameObject penButtonCover;
    [SerializeField] GameObject fillButtonCover;
    [SerializeField] GameObject lineButtonCover;
    [SerializeField] GameObject circleButtonCover;
    [SerializeField] GameObject rectangleButtonCover;

    private void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            PhotonManager.instance.OnLeaveRoomAndDestroy();
        });
    }

    private void Update()
    {
        if (DrawingManager.instance.isDrawable)
        {
            dotUI.SetActive(true);
        }
        else
        {
            dotUI.SetActive(false);
        }

        if (DrawingManager.instance.undoStackCount > 1)
        {
            undoButton.interactable = true;
        }
        else
        {
            undoButton.interactable = false;
        }

        if (DrawingManager.instance.redoStackCount > 0)
        {
            redoButton.interactable = true;
        }
        else
        {
            redoButton.interactable = false;
        }

        if (DrawingManager.instance.HasDrawing())
        {
            clearButton.interactable = true;
        }
        else
        {
            clearButton.interactable = false;
        }

        if (DrawingManager.instance.currentMode == DrawingManager.ToolMode.Pen)
        {
            penButtonCover.SetActive(true);
        }
        else
        {
            penButtonCover.SetActive(false);
        }

        if (DrawingManager.instance.currentMode == DrawingManager.ToolMode.Fill)
        {
            fillButtonCover.SetActive(true);
        }
        else
        {
            fillButtonCover.SetActive(false);
        }

        if (DrawingManager.instance.currentMode == DrawingManager.ToolMode.Line)
        {
            lineButtonCover.SetActive(true);
        }
        else
        {
            lineButtonCover.SetActive(false);
        }

        if (DrawingManager.instance.currentMode == DrawingManager.ToolMode.Circle)
        {
            circleButtonCover.SetActive(true);
        }
        else
        {
            circleButtonCover.SetActive(false);
        }

        if (DrawingManager.instance.currentMode == DrawingManager.ToolMode.Rectrangle)
        {
            rectangleButtonCover.SetActive(true);
        }
        else
        {
            rectangleButtonCover.SetActive(false);
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
                DrawingManager.instance.ChangeMode(DrawingManager.ToolMode.Rectrangle);
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
