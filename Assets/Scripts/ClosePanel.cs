using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    public GameObject panelToClose;

    public void onClick()
    {
        // �p�l�����\���ɂ���
        if (panelToClose != null)
        {
            panelToClose.SetActive(false);
        }
    }
}
