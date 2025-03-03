using UnityEngine;
using UnityEngine.UI;

public class CurrentColor : MonoBehaviour
{
    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        image.color = DrawingManager.instance.drawColor;
    }
}
