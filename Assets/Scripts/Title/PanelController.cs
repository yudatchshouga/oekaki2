using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField] LobbyDrawing lobbyDrawing;
    [SerializeField] Panels currentPanel;

    private enum Panels
    {
        Title,
        Option,
        SizeSetting,
        OnlineMenu,
        Lobby,
        GameSelect,
    }


    public void Start()
    {
        switch (currentPanel)
        {
            case Panels.Title:
                transform.localPosition = new Vector3(0, 0, 0);
                break;
            case Panels.Option:
                transform.localPosition = new Vector3(0, -1500, 0);
                break;
            case Panels.SizeSetting:
                transform.localPosition = new Vector3(-2500, 0, 0);
                break;
            case Panels.OnlineMenu:
                transform.localPosition = new Vector3(0, 1500, 0);
                break;
            case Panels.Lobby:
                transform.localPosition = new Vector3(0, 3000, 0);
                break;
            case Panels.GameSelect:
                transform.localPosition = new Vector3(-2500, 3000, 0);
                break;
        }
    }

    public void OnClickButton(int panel)
    {
        currentPanel = (Panels)panel;
        switch (currentPanel)
        {
            case Panels.Title:
                transform.localPosition = new Vector3(0, 0, 0);
                break;
            case Panels.Option:
                transform.localPosition = new Vector3(0, -1500, 0);
                break;
            case Panels.SizeSetting:
                transform.localPosition = new Vector3(-2500, 0, 0);
                break;
            case Panels.OnlineMenu:
                transform.localPosition = new Vector3(0, 1500, 0);
                break;
            case Panels.Lobby:
                transform.localPosition = new Vector3(0, 3000, 0);
                break;
            case Panels.GameSelect:
                transform.localPosition = new Vector3(-2500, 3000, 0);
                break;
        }
        lobbyDrawing.ClearCanvas();
    }
}
