using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    Texture2D gridTexture;
    [SerializeField] RawImage gridImage;
    [SerializeField] int gridSizeWidth;
    [SerializeField] int gridSizeHeight;
    Color clearColor = new Color(0, 0, 0, 0);
    Color gridColor = new Color(0, 0, 0, 1);

    private void Start()
    {
        gridSizeWidth = DrawingManager.instance.CanvasWidth;
        gridSizeHeight = DrawingManager.instance.CanvasHeight;

        gridTexture = new Texture2D(gridSizeWidth * 30, gridSizeHeight * 30, TextureFormat.RGBA32, false);
        gridTexture.filterMode = FilterMode.Point;
        gridImage.texture = gridTexture;

        CreateGrid();
    }

    public void CreateGrid()
    {
        for (int x = 0; x < gridTexture.width; x++)
        {
            for (int y = 0; y < gridTexture.height; y++)
            {
                if (x % (gridSizeWidth * 3) == 0 || y % (gridSizeHeight * 3) == 0)
                {
                    gridTexture.SetPixel(x, y, gridColor);
                }
                else
                {
                    gridTexture.SetPixel(x, y, clearColor);
                }
            }
        }
        gridTexture.Apply();
    }
}
