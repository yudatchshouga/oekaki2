using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickChecker : MonoBehaviour
{
    //public GameObject panel;
    public GameObject left;
    public GameObject right;
    public GameObject top;
    public GameObject bottom;

    public bool IsClickedFrame()
    {
        // Raycast ‚ÌŒ‹‰Ê‚ğ•Û‚·‚éƒŠƒXƒg
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject == left || result.gameObject == right || result.gameObject == top || result.gameObject == bottom)
            {
                return true;
            }
        }
        return false;
    }
}
