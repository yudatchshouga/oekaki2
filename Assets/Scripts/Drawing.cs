using UnityEngine;

public class Drawing : MonoBehaviour
{
    public Camera cam;
    public GameObject brushPrefab;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(brushPrefab, mousePosition, Quaternion.identity);
        }
    }
}
