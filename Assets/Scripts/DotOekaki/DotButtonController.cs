using UnityEngine;

public class DotButtonController : MonoBehaviour
{
    [SerializeField] GameObject undoButtonCover;
    [SerializeField] GameObject redoButtonCover;
    [SerializeField] GameObject penButtonCover;
    [SerializeField] GameObject fillButtonCover;
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


    // Undo, Redoボタン
    public void OnClickUndoButton()
    {
        DrawingManager.instance.Undo();
    }

    public void OnClickRedoButton()
    {
        DrawingManager.instance.Redo();
    }


    // ツールボタン各種
    public void OnClickPenButton()
    {
        DrawingManager.instance.currentMode = DrawingManager.ToolMode.Pen;
    }
    public void OnClickFillButton()
    {
        DrawingManager.instance.currentMode = DrawingManager.ToolMode.Fill;
    }
    public void OnClickLineButton()
    {
        DrawingManager.instance.currentMode = DrawingManager.ToolMode.Line;
    }
    public void OnClickCircleButton()
    {
        DrawingManager.instance.currentMode = DrawingManager.ToolMode.Circle;
    }
    public void OnClickRectangleButton()
    {
        DrawingManager.instance.currentMode = DrawingManager.ToolMode.Rectrangle;
    }


    // ペンの色変更ボタン各種
    public void OnClickAllClearButton()
    {
        DrawingManager.instance.ClearCanvas();
    }

    public void OnClickBlack()
    {
        DrawingManager.instance.drawColor = Color.black;
    }
    public void OnClickRed()
    { 
        DrawingManager.instance.drawColor = Color.red;
    }
    public void OnClickBlue()
    {
        DrawingManager.instance.drawColor = Color.blue;
    }
    public void OnClickGreen()
    {
        DrawingManager.instance.drawColor = Color.green;
    }
    public void OnClickYellow()
    {
        DrawingManager.instance.drawColor = Color.yellow;
    }
    public void OnClickMagenta()
    {
        DrawingManager.instance.drawColor = Color.magenta;
    }
    public void OnClickCyan()
    {
        DrawingManager.instance.drawColor = Color.cyan;
    }
    public void OnClickGray()
    {
        DrawingManager.instance.drawColor = Color.gray;
    }
    public void OnClickBeige()
    {
        DrawingManager.instance.drawColor = new Color32(246, 184, 148, 255);
    }
    public void OnClickWhite() 
    {
        DrawingManager.instance.drawColor = Color.white;
    }
}
