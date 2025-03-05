using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LineToolPreview : MonoBehaviour
{
    Texture2D previewTexture;
    [SerializeField] RawImage previewPanel;
    [SerializeField] LineRenderer previewLine;
    Color previewLineColor; // ペンの色
    int previewBrushSize; // ブラシの大きさ
    Vector2Int? previewStartPoint = null; // 直線モードの始点

    private void Start()
    {
        int width = DrawingManager.instance.CanvasWidth;
        int height = DrawingManager.instance.CanvasHeight;

        previewTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        previewTexture.filterMode = FilterMode.Point;
        Color[] fill = new Color[width * height];
        for (int i = 0; i < fill.Length; i++)
        {
            fill[i] = Color.white;
        }
        previewTexture.SetPixels(fill);
        previewTexture.Apply();
        previewPanel.texture = previewTexture;

        previewLineColor = DrawingManager.instance.drawColor;
        previewBrushSize = DrawingManager.instance.brushSize;

        previewLine.positionCount = 2;
        previewLine.startColor = previewLineColor;
        previewLine.endColor = previewLineColor;
        previewLine.startWidth = previewBrushSize;
        previewLine.endWidth = previewBrushSize;
        previewLine.enabled = false;
    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(previewPanel.rectTransform, Input.mousePosition, null, out localPoint);

        Rect rect = previewPanel.rectTransform.rect;
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * previewTexture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * previewTexture.height);
        Vector2Int pixelPos = new Vector2Int(x, y);

        if (x < 0 || x >= previewTexture.width || y < 0 || y >= previewTexture.height)
        {
            return;
        }

        if (previewStartPoint != null)
        {
            Vector3 worldStartPoint = CanvasPixelToWorld(previewStartPoint.Value);
            Vector3 worldEndPoint = CanvasPixelToWorld(pixelPos);
            previewLine.SetPosition(0, worldStartPoint);
            previewLine.SetPosition(1, worldEndPoint);
            previewLine.enabled = true;
        }
        else
        { 
            previewLine.enabled = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // 一回目のクリックで始点を設定
            if (previewStartPoint == null)
            {
                previewStartPoint = pixelPos;
            }
            else
            {
                previewStartPoint = null;
                previewLine.enabled = false;
            }
        }
    }

    private Vector3 CanvasPixelToWorld(Vector2Int pixelPos)
    {
        Rect rect = previewPanel.rectTransform.rect;
        float localX = (float)pixelPos.x / previewTexture.width * rect.width;
        float localY = (float)pixelPos.y / previewTexture.height * rect.height;
        Vector3 localPos = new Vector3(localX, localY, 0);
        return previewPanel.rectTransform.TransformPoint(localPos);
    }
}
