using UnityEngine;

public class DotButtonController : MonoBehaviour
{
    [SerializeField] GameObject undoButtonCover;
    [SerializeField] GameObject redoButtonCover;

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
    }

    public void OnClickUndoButton()
    {
        DrawingManager.instance.Undo();
    }

    public void OnClickRedoButton()
    {
        DrawingManager.instance.Redo();
    }

    public void OnClickAllClearButton()
    {
        DrawingManager.instance.ClearCanvas();
    }

    public void OnClickBlack()
    {
        DrawingManager.instance.ColorIndex = 0;
    }
    public void OnClickRed()
    { 
        DrawingManager.instance.ColorIndex = 1;
    }
    public void OnClickBlue()
    {
        DrawingManager.instance.ColorIndex = 2;
    }
    public void OnClickGreen()
    {
        DrawingManager.instance.ColorIndex = 3;
    }
    public void OnClickYellow()
    {
        DrawingManager.instance.ColorIndex = 4;
    }
    public void OnClickMagenta()
    {
        DrawingManager.instance.ColorIndex = 5;
    }
    public void OnClickCyan()
    {
        DrawingManager.instance.ColorIndex = 6;
    }
    public void OnClickGray()
    {
        DrawingManager.instance.ColorIndex = 7;
    }
    public void OnClickBeige()
    {
        DrawingManager.instance.ColorIndex = 8;
    }
    public void OnClickWhite() 
    {
        DrawingManager.instance.ColorIndex = 9;
    }
}
