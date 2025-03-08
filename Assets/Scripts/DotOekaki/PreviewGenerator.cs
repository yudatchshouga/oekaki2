using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static DrawingManager;

public class PreviewGenerator : MonoBehaviour
{
    Texture2D previewTexture;
    [SerializeField] RawImage previewPanel;
    Vector2Int? startPoint = null; // 直線モードの始点
    Vector2Int startPixel; // 円モード、長方形モードの始点
    bool isDrawing = false; // 描画中かどうか
    Color previewColor;
    ToolMode currentPreviewMode;
    int previewBrushSize;
    DrawingUtils drawer;

    private void Start()
    {
        int previewWidth = DrawingManager.instance.CanvasWidth;
        int previewHeight = DrawingManager.instance.CanvasHeight;

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

        previewColor = DrawingManager.instance.drawColor;
        previewBrushSize = DrawingManager.instance.brushSize;
        currentPreviewMode = DrawingManager.instance.currentMode;
        drawer = new DrawingUtils(previewTexture, previewColor, previewBrushSize);

        if (Input.GetMouseButtonDown(0))
        {
            if (currentPreviewMode == ToolMode.Circle || currentPreviewMode == ToolMode.Rectrangle)
            {
                if (!IsInsideCanvas(localPoint))
                {
                    return;
                }
                Vector2Int pixelPos = new Vector2Int(x, y);
                // 始点を設定
                if (!isDrawing)
                {
                    startPixel = pixelPos;
                    isDrawing = true;
                }
            }
        }

        if (IsInsideCanvas(localPoint))
        {
            if (currentPreviewMode == ToolMode.Line)
            {
                if (isDrawing)
                {
                    Vector2Int endPoint = new Vector2Int(x, y);
                    DrawShape(startPoint.Value, endPoint);
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (currentPreviewMode == ToolMode.Circle || currentPreviewMode == ToolMode.Rectrangle)
            {
                if (isDrawing)
                { 
                    Vector2Int endPixel = new Vector2Int(x, y);
                    DrawShape(startPixel, endPixel);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentPreviewMode == ToolMode.Line)
            {
                if (!isDrawing)
                {
                    Vector2Int pixelPos = new Vector2Int(x, y);
                    if (x < 0 || x >= previewTexture.width || y < 0 || y >= previewTexture.height)
                    {
                        return;
                    }
                    DrawPoint(pixelPos);
                    startPoint = pixelPos;
                    isDrawing = true;
                }
                else
                {
                    if (!IsInsideCanvas(localPoint))
                    {
                        return;
                    }
                    ClearCanvas();
                    isDrawing = false;
                }
            }
            else if (currentPreviewMode == ToolMode.Circle || currentPreviewMode == ToolMode.Rectrangle)
            {
                if (isDrawing)
                {
                    ClearCanvas();
                    isDrawing = false;
                }
            }
        }
    }


    private void DrawPoint(Vector2Int position)
    {
        drawer.DrawPoint(position);
        previewTexture.Apply();
    }

    private void DrawShape(Vector2Int start, Vector2Int end)
    {
        if (currentPreviewMode == ToolMode.Line)
        {
            DrawLine(start, end);
        }
        else if (currentPreviewMode == ToolMode.Circle)
        {
            DrawCircle(start, end);
        }
        else if (currentPreviewMode == ToolMode.Rectrangle)
        {
            DrawRectangle(start, end);
        }
    }

    // Bresenhamの直線アルゴリズム
    private void DrawLine(Vector2Int start, Vector2Int end)
    {
        ClearCanvas();
        drawer.DrawLine(start, end);
        previewTexture.Apply();
    }

    // Bresenhamの楕円アルゴリズム
    private void DrawCircle(Vector2Int start, Vector2Int end)
    {
        ClearCanvas();
        drawer.DrawCircle(start, end);
        previewTexture.Apply();
    }

    private void DrawRectangle(Vector2Int start, Vector2Int end)
    {
        ClearCanvas();
        drawer.DrawRectangle(start, end);
        previewTexture.Apply();
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

    private bool IsInsideCanvas(Vector2 localPoint)
    {
        Rect rect = previewPanel.rectTransform.rect;
        return localPoint.x >= rect.x && localPoint.x <= rect.x + rect.width
            && localPoint.y >= rect.y && localPoint.y <= rect.y + rect.height;
    }
}
