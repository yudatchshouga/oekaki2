using UnityEngine;

public class PanelController : MonoBehaviour
{
    public void OnClickOfflineButton()
    {
        transform.localPosition = new Vector3(-2000, 0, 0);
    }

    public void OnClickOnlineButton()
    {
        transform.localPosition = new Vector3(0, 1500, 0);
    }

    public void OnClickBackButton()
    {
        transform.localPosition = new Vector3(0, 0, 0);
    }
}
