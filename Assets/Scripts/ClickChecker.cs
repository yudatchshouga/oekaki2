using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickChecker : MonoBehaviour
{
    public GameObject panel; // Panel の参照を Inspector で設定

    //void Update()
    //{
    //    // マウスの左クリックを判定
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        // Panel の外側をクリックしたかを判定
    //        bool isOutsidePanel = IsClickedOutsidePanel();
    //        Debug.Log("Clicked Outside Panel: " + isOutsidePanel);
    //    }
    //}

    // === 修正点: Panel の外側をクリックしたかを判定するメソッド ===
    public bool IsClickedOutsidePanel()
    {
        // Raycast の結果を保持するリスト
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        // Raycast の結果に Panel が含まれているかを確認
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject == panel || result.gameObject.transform.IsChildOf(panel.transform))
            {
                // Panel またはその子オブジェクトがクリックされた場合
                return false;
            }
        }

        // Panel の外側がクリックされた場合
        return true;
    }
}
