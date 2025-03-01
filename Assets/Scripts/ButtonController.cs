using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] LineDrawing lineDrawing;
    [SerializeField] GameObject palettePanel;
    [SerializeField] GameObject openButton;
    [SerializeField] GameObject fillButton;
    [SerializeField] GameObject fillButtonBackground;

    public void onClickBlack()
    {
        lineDrawing.index = 0;
    }
    public void onClickYellow()
    {
        lineDrawing.index = 1;
    }
    public void onClickRed()
    {
        lineDrawing.index = 2;
    }
    public void onClickBlue()
    {
        lineDrawing.index = 3;
    }
    public void onClickGreen() 
    {
        lineDrawing.index = 4;
    }

    public void onClickOpenPanelButton()
    {
        palettePanel.SetActive(true);
        openButton.SetActive(false);
    }

    public void onClickClosePanelButton()
    {
        palettePanel.SetActive(false);
        openButton.SetActive(true);
    }
    
    public void onClickFillButton()
    {
        if (!FillTool.instance.isFillMode)
        {
            FillTool.instance.isFillMode = true;
            fillButtonBackground.SetActive(true);
        }
        else
        {
            FillTool.instance.isFillMode = false;
            fillButtonBackground.SetActive(false);
        }
    }
}

