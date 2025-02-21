using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] LineDrawing lineDrawing;

    public void onClickYellow() 
    {
        lineDrawing.index = 1;
    }
}
