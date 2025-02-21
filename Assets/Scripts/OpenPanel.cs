using UnityEngine;

public class OpenPanel : MonoBehaviour
{
    public GameObject panel;

    public void onClick()
    {
        // ƒpƒlƒ‹‚ð”ñ•\Ž¦‚É‚·‚é
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }
}
