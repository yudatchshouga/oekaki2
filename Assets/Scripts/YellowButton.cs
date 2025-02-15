using UnityEngine;

public class YellowButton : MonoBehaviour
{
    [SerializeField] LineDrawing lineDrawing;

    public void onClick() 
    {
        lineDrawing.currentLineRenderer.startColor = Color.yellow;
        lineDrawing.currentLineRenderer.endColor = Color.yellow;
    }
}
