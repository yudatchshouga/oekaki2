using UnityEngine;
using UnityEngine.UI;

public class GridGeneratorOff : MonoBehaviour
{
    Texture2D gridTexture;
    [SerializeField] RawImage gridPanel;
    [SerializeField] Toggle gridToggle;
    [SerializeField] int gridSize;
    Color clearColor;
    Color gridColor;

    int gridSizeWidth;
    int gridSizeHeight;

    private void Start()
    {
        clearColor = new Color(0, 0, 0, 0);
        gridColor = new Color(51f / 255f, 51f / 255f, 51f / 255f, 1);


        gridSizeWidth = DrawingManagerOff.instance.CanvasWidth * gridSize;
        gridSizeHeight = DrawingManagerOff.instance.CanvasHeight * gridSize;

        CreateTexture(gridSizeWidth, gridSizeHeight);
    }

    private void CreateTexture(int width, int height)
    {
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

    private void CreateGrid(int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x % gridSize == 0 || x % gridSize == gridSize - 1 || x == 1 || x == width - 2 || y % gridSize == 0 || y % gridSize == gridSize - 1 || y == 1 || y == height - 2)
                {
                    gridTexture.SetPixel(x, y, gridColor);
                }
            }
        }

        if (DrawingManagerOff.instance.CanvasWidth > 10 || DrawingManagerOff.instance.CanvasHeight > 10)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x % gridSize == 1 || y % gridSize == 1 || x == 2 || y == 2 || x == width - 3 || y == height - 3)
                    {
                        gridTexture.SetPixel(x, y, gridColor);
                    }
                }
            }
        }

        if (DrawingManagerOff.instance.CanvasWidth > 20 || DrawingManagerOff.instance.CanvasHeight > 20)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x % gridSize == gridSize - 2 || y % gridSize == gridSize - 2 || x == 3 || y == 3 || x == width - 4 || y == width - 4)
                    {
                        gridTexture.SetPixel(x, y, gridColor);
                    }
                }
            }
        }

        if (DrawingManagerOff.instance.CanvasWidth > 30 || DrawingManagerOff.instance.CanvasHeight > 30)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x % gridSize == 2 || y % gridSize == 2 || x == 4 || y == 4 || x == width - 5 || y == height - 5)
                    {
                        gridTexture.SetPixel(x, y, gridColor);
                    }
                }
            }
        }

        if (DrawingManagerOff.instance.CanvasWidth > 40 || DrawingManagerOff.instance.CanvasHeight > 40)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x % gridSize == gridSize - 3 || y % gridSize == gridSize - 3 || x == 5 || y == 5 || x == width - 6 || y == height - 6)
                    {
                        gridTexture.SetPixel(x, y, gridColor);
                    }
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

            gridSizeWidth = DrawingManagerOff.instance.CanvasWidth * gridSize;
            gridSizeHeight = DrawingManagerOff.instance.CanvasHeight * gridSize;

            CreateTexture(gridSizeWidth, gridSizeHeight);
            CreateGrid(gridTexture.width, gridTexture.height);
        }
        else
        {
            gridPanel.enabled = false;
        }
    }

    public void ChangeInteractableGridToggle(bool isInteractable)
    {
        gridToggle.interactable = isInteractable;
    }
}
