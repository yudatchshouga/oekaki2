using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField] LobbyDrawing lobbyDrawing;
    [SerializeField] Panels currentPanel;

    private enum Panels
    {
        Title,
        SizeSelect,
        OnlineMenu,
        Lobby,
    }

    //private Panels currentPanel;

    public void Start()
    {
        //currentPanel = Panels.Title;
        OnClickButton((int)currentPanel);
    }

    public void OnClickButton(int panel)
    {
        currentPanel = (Panels)panel;
        switch (currentPanel)
        {
            case Panels.Title:
                transform.localPosition = new Vector3(0, 0, 0);
                break;
            case Panels.SizeSelect:
                transform.localPosition = new Vector3(-2000, 0, 0);
                break;
            case Panels.OnlineMenu:
                transform.localPosition = new Vector3(0, 1500, 0);
                break;
            case Panels.Lobby:
                transform.localPosition = new Vector3(0, 3000, 0);
                break;
        }
        lobbyDrawing.ClearCanvas();
    }
}
