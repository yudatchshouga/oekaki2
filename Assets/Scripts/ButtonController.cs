using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] LineDrawing lineDrawing;

    public void onClick() 
    {
        lineDrawing.index = 1;
    }
}
