using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    Texture2D gridTexture;
    [SerializeField] RawImage gridPanel;
    [SerializeField] Toggle gridToggle;
    [SerializeField] int gridSize;
    [SerializeField] int gridThickness; // ÉOÉäÉbÉhê¸ÇÃëæÇ≥
    Color clearColor = new Color(0, 0, 0, 0);
    Color gridColor = new Color(0, 0, 0, 0.92f);

    private void Start()
    {
        int gridSizeWidth = DrawingManager.instance.CanvasWidth * gridSize;
        int gridSizeHeight = DrawingManager.instance.CanvasHeight * gridSize;

        gridTexture = new Texture2D(gridSizeWidth, gridSizeHeight, TextureFormat.RGBA32, false);
        gridTexture.filterMode = FilterMode.Point;

        Color[] colors = new Color[gridSizeWidth * gridSizeHeight];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = clearColor;
        }
        gridTexture.SetPixels(colors);
        gridTexture.Apply();
        gridPanel.texture = gridTexture;

        CreateGrid(gridSizeWidth, gridSizeHeight);
    }

    private void CreateGrid(int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x % gridSize < gridThickness || y % gridSize < gridThickness || x >= width - gridThickness || y >= height - gridThickness)
                {
                    gridTexture.SetPixel(x, y, gridColor);
                }
            }
        }
        gridTexture.Apply();
    }

    public void ToggleGrid()
    {
        if (gridToggle.isOn)
        {
            gridPanel.enabled = true;
        }
        else
        {
            gridPanel.enabled = false;
        }
    }
}
