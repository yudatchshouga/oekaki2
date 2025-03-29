using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    Texture2D gridTexture;
    [SerializeField] RawImage gridPanel;
    [SerializeField] Toggle gridToggle;
    [SerializeField] int gridSize;
    [SerializeField] int gridThickness; // グリッド線の太さ
    Color clearColor;
    Color gridColor;

    int gridSizeWidth;
    int gridSizeHeight;

    private void Start()
    {
        clearColor = new Color(0, 0, 0, 0);
        gridColor = new Color(51f / 255f, 51f / 255f, 51f / 255f, 1);

        gridSizeWidth = DrawingManager.instance.CanvasWidth * gridSize;
        gridSizeHeight = DrawingManager.instance.CanvasHeight * gridSize;

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
    }

    public void CreateGrid(int width, int height)
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
            CreateGrid(gridTexture.width, gridTexture.height);
        }
        else
        {
            gridPanel.enabled = false;
        }
    }
}
