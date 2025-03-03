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
        switch (DrawingManager.instance.ColorIndex)
        {
            case 0:
                image.color = Color.black;
                break;
            case 1:
                image.color = Color.red;
                break;
            case 2:
                image.color = Color.blue;
                break;
            case 3:
                image.color = Color.green;
                break;
            case 4:
                image.color = Color.yellow;
                break;
            case 5:
                image.color = Color.magenta;
                break;
            case 6:
                image.color = Color.cyan;
                break;
            case 7:
                image.color = Color.gray;
                break;
            case 8:
                image.color = new Color32(246, 184, 148, 255);
                break;
            case 9:
                image.color = Color.white;
                break;
            default:
                image.color = Color.black;
                break;
        }
    }
}
