using UnityEngine;

public class OpenPanel : MonoBehaviour
{
    public GameObject panel;

    public void onClick()
    {
        // �p�l�����\���ɂ���
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }
}
