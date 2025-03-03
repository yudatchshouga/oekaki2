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
