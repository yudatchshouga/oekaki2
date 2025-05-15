using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField] LobbyDrawing lobbyDrawing;
    [SerializeField] Panels currentPanel;

    private enum Panels
    {
        Title,
        Option,
        Offline,
        OnlineMenu,
        Lobby,
        GameSelect,
        QuizSetting,
        CooperationSetting,
        EsiritoriSetting,
        YonkomaSetting,
        DengonSetting,
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
            case Panels.Offline:
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
            case Panels.QuizSetting:
                transform.localPosition = new Vector3(-5000, 3000, 0);
                break;
            case Panels.CooperationSetting:
                transform.localPosition = new Vector3(-5000, 4500, 0);
                break;
            case Panels.EsiritoriSetting:
                transform.localPosition = new Vector3(-5000, 6000, 0);
                break;
            case Panels.YonkomaSetting:
                transform.localPosition = new Vector3(-5000, 7500, 0);
                break;
            case Panels.DengonSetting:
                transform.localPosition = new Vector3(-5000, 9000, 0);
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
            case Panels.Offline:
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
            case Panels.QuizSetting:
                transform.localPosition = new Vector3(-5000, 3000, 0);
                break;
            case Panels.CooperationSetting:
                transform.localPosition = new Vector3(-5000, 4500, 0);
                break;
            case Panels.EsiritoriSetting:
                transform.localPosition = new Vector3(-5000, 6000, 0);
                break;
            case Panels.YonkomaSetting:
                transform.localPosition = new Vector3(-5000, 7500, 0);
                break;
            case Panels.DengonSetting:
                transform.localPosition = new Vector3(-5000, 9000, 0);
                break;
        }
        lobbyDrawing.ClearCanvas();
    }
}
