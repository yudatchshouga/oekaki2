using UnityEngine;

public class YellowButton : MonoBehaviour
{
    [SerializeField] LineDrawing lineDrawing;

    public void onClick() 
    {
        lineDrawing.index = 1;
    }
}
