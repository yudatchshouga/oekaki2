using UnityEngine;
using UnityEngine.UI;

public class LobbyDrawing : MonoBehaviour
{
    Texture2D texture;
    [SerializeField] RawImage rawImage;
    Color drawColor;
    int penSize;
    int CanvasWidth;
    int CanvasHeight;
    Vector2Int? lastPoint = null;
    DrawingUtils drawer;

    private void Start()
    {
        CanvasWidth = 100;
        CanvasHeight = 50;
        texture = new Texture2D(CanvasWidth, CanvasHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        rawImage.texture = texture;
        drawColor = Color.black;
        penSize = 1;

        ClearCanvas();
    }

    void Update()
    {
        drawer = new DrawingUtils(texture, drawColor, penSize);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImage.rectTransform, Input.mousePosition, null, out localPoint);

        Rect rect = rawImage.rectTransform.rect;
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        if (Input.GetMouseButton(0))
        {
            DrawAtPoint(localPoint);
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastPoint = null;
        }
    }

    private void DrawAtPoint(Vector2 localPoint)
    {
        Rect rect = rawImage.rectTransform.rect;

        // ÉçÅ[ÉJÉãç¿ïWÇTexture2DÇÃç¿ïWÇ…ïœä∑
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        if (lastPoint.HasValue)
        {
            drawer.DrawLine(lastPoint.Value, new Vector2Int(x, y)); // 2âÒñ⁄à»ç~ÇÃï`âÊ
        }
        else
        {
            drawer.DrawPoint(new Vector2Int(x, y)); // ç≈èâÇÃï`âÊ
        }

        lastPoint = new Vector2Int(x, y);
        texture.Apply();
    }

    public void ClearCanvas()
    {
        Color[] colors = new Color[CanvasWidth * CanvasHeight];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        texture.SetPixels(colors);
        texture.Apply();
    }
}
