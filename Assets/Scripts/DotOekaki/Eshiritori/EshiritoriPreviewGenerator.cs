using UnityEngine;
using UnityEngine.UI;

public class EshiritoriPreviewGenerator : MonoBehaviour
{
    Texture2D previewTexture;
    [SerializeField] RawImage previewPanel;
    Vector2Int? startPoint = null; // 直線モードの始点
    Vector2Int startPixel; // 円モード、長方形モードの始点
    bool isDrawing = false; // 描画中かどうか
    Color previewColor;
    int previewBrushSize;
    DrawingUtils drawer;

    private void Start()
    {
        int previewWidth = EshiritoriDrawingManager.instance.CanvasWidth;
        int previewHeight = EshiritoriDrawingManager.instance.CanvasHeight;

        previewTexture = new Texture2D(previewWidth, previewHeight, TextureFormat.RGBA32, false);
        previewTexture.filterMode = FilterMode.Point;
        ClearCanvas();
        previewPanel.texture = previewTexture;
    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(previewPanel.rectTransform, Input.mousePosition, null, out localPoint);

        Rect rect = previewPanel.rectTransform.rect;
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * previewTexture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * previewTexture.height);

        previewColor = EshiritoriDrawingManager.instance.drawColor;
        previewBrushSize = EshiritoriDrawingManager.instance.brushSize;
        drawer = new DrawingUtils(previewTexture, previewColor, previewBrushSize);
    }

    private void ClearCanvas()
    {
        Color[] colors = new Color[previewTexture.width * previewTexture.height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(0, 0, 0, 0);
        }
        previewTexture.SetPixels(colors);
        previewTexture.Apply();
    }
}
