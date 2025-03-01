using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] LineManager lineManager;
    [SerializeField] GameObject palettePanel;
    [SerializeField] GameObject openButton;
    [SerializeField] GameObject fillButton;
    [SerializeField] GameObject fillButtonBackground;

    public void onClickBlack()
    {
        lineManager.index = 0;
    }
    public void onClickRed()
    {
        lineManager.index = 1;
    }
    public void onClickGreen()
    {
        lineManager.index = 2;
    }
    public void onClickBlue()
    {
        lineManager.index = 3;
    }
    public void onClickYellow()
    {
        lineManager.index = 4;
    }
    public void onClickCyan()
    {
        lineManager.index = 5;
    }
    public void onClickMagenta() 
    {
        lineManager.index = 6;
    }
    public void onClickGray()
    {
        lineManager.index = 7;
    }
    public void onClickWhite()
    {
        lineManager.index = 8;
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

