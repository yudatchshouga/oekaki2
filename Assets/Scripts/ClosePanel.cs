using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    public GameObject panelToClose;

    public void onClick()
    {
        // ƒpƒlƒ‹‚ð”ñ•\Ž¦‚É‚·‚é
        if (panelToClose != null)
        {
            panelToClose.SetActive(false);
        }
    }
}
