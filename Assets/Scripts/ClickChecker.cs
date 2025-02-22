using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickChecker : MonoBehaviour
{
    public GameObject panel; // Panel �̎Q�Ƃ� Inspector �Őݒ�

    //void Update()
    //{
    //    // �}�E�X�̍��N���b�N�𔻒�
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        // Panel �̊O�����N���b�N�������𔻒�
    //        bool isOutsidePanel = IsClickedOutsidePanel();
    //        Debug.Log("Clicked Outside Panel: " + isOutsidePanel);
    //    }
    //}

    // === �C���_: Panel �̊O�����N���b�N�������𔻒肷�郁�\�b�h ===
    public bool IsClickedOutsidePanel()
    {
        // Raycast �̌��ʂ�ێ����郊�X�g
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        // Raycast �̌��ʂ� Panel ���܂܂�Ă��邩���m�F
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject == panel || result.gameObject.transform.IsChildOf(panel.transform))
            {
                // Panel �܂��͂��̎q�I�u�W�F�N�g���N���b�N���ꂽ�ꍇ
                return false;
            }
        }

        // Panel �̊O�����N���b�N���ꂽ�ꍇ
        return true;
    }
}
