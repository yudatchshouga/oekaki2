using UnityEngine;

public class DotButtonController : MonoBehaviour
{
    [SerializeField] GameObject undoButtonCover;
    [SerializeField] GameObject redoButtonCover;
    [SerializeField] GameObject penButtonCover;
    [SerializeField] GameObject fillButtonCover;
    [SerializeField] GameObject spoitButtonCover;
    [SerializeField] GameObject lineButtonCover;
    [SerializeField] GameObject circleButtonCover;
    [SerializeField] GameObject rectangleButtonCover;


    private void Update()
    {
        if (DrawingManager.instance.undoStackCount > 1)
        {
            undoButtonCover.SetActive(false);
        }
        else
        {
            undoButtonCover.SetActive(true);
        }

        if (DrawingManager.instance.redoStackCount > 0)
        {
            redoButtonCover.SetActive(false);
        }
        else
        {
            redoButtonCover.SetActive(true);
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

        if (DrawingManager.instance.currentMode == DrawingManager.ToolMode.Spoit)
        {
            spoitButtonCover.SetActive(true);
        }
        else
        {
            spoitButtonCover.SetActive(false);
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


    // Undo, Redo�{�^��
    public void OnClickUndoButton()
    {
        DrawingManager.instance.Undo();
    }

    public void OnClickRedoButton()
    {
        DrawingManager.instance.Redo();
    }


    // �c�[���{�^���e��
    public void OnClickPenButton()
    {
        DrawingManager.instance.ChangeMode(DrawingManager.ToolMode.Pen);
    }
    public void OnClickFillButton()
    {
        DrawingManager.instance.ChangeMode(DrawingManager.ToolMode.Fill);
    }
    public void OnClickSpoitButtton()
    {
        DrawingManager.instance.ChangeMode(DrawingManager.ToolMode.Spoit);
    }
    public void OnClickLineButton()
    {
        DrawingManager.instance.ChangeMode(DrawingManager.ToolMode.Line);
    }
    public void OnClickCircleButton()
    {
        DrawingManager.instance.ChangeMode(DrawingManager.ToolMode.Circle);
    }
    public void OnClickRectangleButton()
    {
        DrawingManager.instance.ChangeMode(DrawingManager.ToolMode.Rectrangle);
    }


    // �y���̐F�ύX�{�^���e��
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
