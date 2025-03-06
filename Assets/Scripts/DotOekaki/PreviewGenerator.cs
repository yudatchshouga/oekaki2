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

        if (Input.GetMouseButtonDown(0))
        {
            if (currentPreviewMode == ToolMode.Circle || currentPreviewMode == ToolMode.Rectrangle)
            {
                if (x < 0 || x >= previewTexture.width || y < 0 || y >= previewTexture.height)
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
                    DrawPoint(pixelPos.x, pixelPos.y);
                    startPoint = pixelPos;
                    isDrawing = true;
                }
                else
                {
                    if (x < 0 || x >= previewTexture.width || y < 0 || y >= previewTexture.height)
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


    private void DrawPoint(int cx, int cy)
    {
        if (previewBrushSize == 1)
        {
            previewTexture.SetPixel(cx, cy, previewColor);
        }
        // ブラシの大きさが偶数の場合と奇数の場合で処理を分ける
        else if (previewBrushSize % 2 == 0)
        {
            int halfSize = previewBrushSize / 2;
            for (int dx = -halfSize + 1; dx <= halfSize; dx++)
            {
                for (int dy = -halfSize + 1; dy <= halfSize; dy++)
                {
                    int px = cx + dx;
                    int py = cy + dy;
                    if (px >= 0 && px < previewTexture.width && py >= 0 && py < previewTexture.height)
                    {
                        previewTexture.SetPixel(px, py, previewColor);
                    }
                }
            }
        }
        else if (previewBrushSize % 2 == 1)
        {
            int halfSize = previewBrushSize / 2;
            for (int dx = -halfSize; dx <= halfSize; dx++)
            {
                for (int dy = -halfSize; dy <= halfSize; dy++)
                {
                    int px = cx + dx;
                    int py = cy + dy;
                    if (px >= 0 && px < previewTexture.width && py >= 0 && py < previewTexture.height)
                    {
                        previewTexture.SetPixel(px, py, previewColor);
                    }
                }
            }
        }
        previewTexture.Apply();
    }

    private void DrawShape(Vector2Int start, Vector2Int end)
    {
        if (currentPreviewMode == ToolMode.Line)
        {
            DrawLine(start.x, start.y, end.x, end.y);
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
    private void DrawLine(int x0, int y0, int x1, int y1)
    {
        ClearCanvas();
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2;

        while (true)
        {
            DrawPoint(x0, y0);
            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
        previewTexture.Apply();
    }

    // Bresenhamの楕円アルゴリズム
    private void DrawCircle(Vector2Int start, Vector2Int end)
    {
        ClearCanvas();
        int centerX = (start.x + end.x) / 2;
        int centerY = (start.y + end.y) / 2;
        int radiusX = Mathf.Abs(centerX - start.x);
        int radiusY = Mathf.Abs(centerY - start.y);

        int x, y;
        float dx, dy, d1, d2;

        x = 0;
        y = radiusY;

        // 第一区間(x増加,y一定)
        d1 = (radiusY * radiusY) - (radiusX * radiusX * radiusY) + (0.25f * radiusX * radiusX);
        dx = 2 * radiusY * radiusY * x;
        dy = 2 * radiusX * radiusX * y;

        while (dx < dy)
        {
            DrawPoint(centerX + x, centerY + y);
            DrawPoint(centerX - x, centerY + y);
            DrawPoint(centerX + x, centerY - y);
            DrawPoint(centerX - x, centerY - y);

            x++;
            dx += 2 * radiusY * radiusY;
            if (d1 < 0)
            {
                d1 += dx + radiusY * radiusY;
            }
            else
            {
                y--;
                dy -= 2 * radiusX * radiusX;
                d1 += dx - dy + radiusY * radiusY;
            }
        }
        // 第二区間(x一定,y減少)
        d2 = ((radiusY * radiusY) * ((x + 0.5f) * (x + 0.5f))) + ((radiusX * radiusX) * ((y - 1) * (y - 1))) - (radiusX * radiusX * radiusY * radiusY);

        while (y >= 0)
        {
            DrawPoint(centerX + x, centerY + y);
            DrawPoint(centerX - x, centerY + y);
            DrawPoint(centerX + x, centerY - y);
            DrawPoint(centerX - x, centerY - y);

            y--;
            dy -= 2 * radiusX * radiusX;
            if (d2 > 0)
            {
                d2 += radiusX * radiusX - dy;
            }
            else
            {
                x++;
                dx += 2 * radiusY * radiusY;
                d2 += dx - dy + radiusX * radiusX;
            }
        }
        previewTexture.Apply();
    }

    private void DrawRectangle(Vector2Int start, Vector2Int end)
    {
        ClearCanvas();
        int xMin = Mathf.Min(start.x, end.x);
        int xMax = Mathf.Max(start.x, end.x);
        int yMin = Mathf.Min(start.y, end.y);
        int yMax = Mathf.Max(start.y, end.y);

        for (int x = xMin; x <= xMax; x++)
        {
            DrawPoint(x, start.y);
            DrawPoint(x, end.y);
        }
        for (int y = yMin; y <= yMax; y++)
        {
            DrawPoint(start.x, y);
            DrawPoint(end.x, y);
        }
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
