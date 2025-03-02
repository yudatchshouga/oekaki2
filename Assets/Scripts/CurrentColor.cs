using UnityEngine;
using UnityEngine.UI;

public class CurrentColor : MonoBehaviour
{
    [SerializeField] LineManager lineManager;
    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (lineManager.index == 0)
        {
            image.color = Color.black;
        }
        else if (lineManager.index == 1)
        {
            image.color = Color.red;
        }
        else if (lineManager.index == 2)
        {
            image.color = Color.green;
        }
        else if (lineManager.index == 3)
        {
            image.color = Color.blue;
        }
        else if (lineManager.index == 4)
        {
            image.color = Color.yellow;
        }
        else if (lineManager.index == 5)
        {
            image.color = Color.cyan;
        }
        else if (lineManager.index == 6)
        {
            image.color = Color.magenta;
        }
        else if (lineManager.index == 7)
        {
            image.color = Color.gray;
        }
        else if (lineManager.index == 8)
        {
            image.color = Color.white;
        }
        else
        {
            image.color = Color.black;
        }
    }
}
