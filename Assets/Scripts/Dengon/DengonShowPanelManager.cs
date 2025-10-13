using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DengonShowPanelManager : MonoBehaviourPun
{
    [SerializeField] GameObject showField;
    [SerializeField] RawImage showingPanel;
    [SerializeField] Toggle gridToggle;

    Texture2D gridTexture;
    [SerializeField] RawImage gridPanel;
    [SerializeField] int gridSize;
    [SerializeField] Color clearColor;
    [SerializeField] Color gridColor;

    [SerializeField] Text fromText;
    [SerializeField] Text toText;

    public void SetShowPanel(Texture texture)
    {
        showingPanel.texture = texture;
    }

    public void SetFromText(string text)
    {
        fromText.text = text;
    }

    public void SetToText(string text)
    {
        toText.text = text;
    }

    private void SetShowFieldSize(int width, int height)
    {
        RectTransform rectTransform = showField.GetComponent<RectTransform>();
        float aspectRatio = (float)width / height;

        if (aspectRatio > 1)
        {
            rectTransform.sizeDelta = new Vector2(900, 900 / aspectRatio);
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(900 * aspectRatio, 900);
        }
    }

    public void ToggleGridShowPanel()
    {
        if (gridToggle.isOn)
        {
            gridPanel.enabled = true;
            CreateGridTexture();
            CreateGrid(showingPanel.texture.width, showingPanel.texture.height, gridTexture);
        }
        else
        {
            gridPanel.enabled = false;
        }
    }

    public void CreateGridTexture()
    {
        int width = showingPanel.texture.width * gridSize;
        int height = showingPanel.texture.height * gridSize;

        gridTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        gridTexture.filterMode = FilterMode.Point;

        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = clearColor;
        }
        gridTexture.SetPixels(colors);
        gridTexture.Apply();
        gridPanel.texture = gridTexture;
    }

    private void CreateGrid(int width, int height, Texture2D texture)
    {
        if (width > 50 || height > 50) return;

        for (int x = 0; x < gridTexture.width; x++)
        {
            for (int y = 0; y < gridTexture.height; y++)
            {
                if (x % gridSize == 0 || x % gridSize == gridSize - 1 || x == 1 || x == gridTexture.width - 2 || y % gridSize == 0 || y % gridSize == gridSize - 1 || y == 1 || y == gridTexture.height - 2)
                {
                    texture.SetPixel(x, y, gridColor);
                }
            }
        }

        if (width > 10 || height > 10)
        {
            for (int x = 0; x < gridTexture.width; x++)
            {
                for (int y = 0; y < gridTexture.height; y++)
                {
                    if (x % gridSize == 1 || y % gridSize == 1 || x == 2 || y == 2 || x == gridTexture.width - 3 || y == gridTexture.height - 3)
                    {
                        texture.SetPixel(x, y, gridColor);
                    }
                }
            }
        }

        if (width > 20 || height > 20)
        {
            for (int x = 0; x < gridTexture.width; x++)
            {
                for (int y = 0; y < gridTexture.height; y++)
                {
                    if (x % gridSize == gridSize - 2 || y % gridSize == gridSize - 2 || x == 3 || y == 3 || x == gridTexture.width - 4 || y == gridTexture.width - 4)
                    {
                        texture.SetPixel(x, y, gridColor);
                    }
                }
            }
        }

        if (width > 30 || height > 30)
        {
            for (int x = 0; x < gridTexture.width; x++)
            {
                for (int y = 0; y < gridTexture.height; y++)
                {
                    if (x % gridSize == 2 || y % gridSize == 2 || x == 4 || y == 4 || x == gridTexture.width - 5 || y == gridTexture.height - 5)
                    {
                        texture.SetPixel(x, y, gridColor);
                    }
                }
            }
        }

        if (width > 40 || height > 40)
        {
            for (int x = 0; x < gridTexture.width; x++)
            {
                for (int y = 0; y < gridTexture.height; y++)
                {
                    if (x % gridSize == gridSize - 3 || y % gridSize == gridSize - 3 || x == 5 || y == 5 || x == gridTexture.width - 6 || y == gridTexture.height - 6)
                    {
                        texture.SetPixel(x, y, gridColor);
                    }
                }
            }
        }
        gridTexture.Apply();
    }
}
