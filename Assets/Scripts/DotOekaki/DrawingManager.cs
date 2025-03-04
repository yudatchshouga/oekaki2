using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;

public class DrawingManager : MonoBehaviour
{
    public static DrawingManager instance;

    Texture2D texture;
    [SerializeField] RawImage drawingPanel;
    public int CanvasWidth; // キャンバスの横幅
    public int CanvasHeight; // キャンバスの縦幅
    public Color drawColor; // ペンの色
    public int brushSize; // ブラシの大きさ
    public float penAlpha; // ペンの透明度(要らないかもしれない)
    Vector2Int? lastPoint = null; // 前回の描画位置
    Stack<Color[]> undoStack; // 元に戻すためのスタック
    Stack<Color[]> redoStack; // やり直しのためのスタック
    public bool isPenMode = true; // ペンモード
    public bool isFillMode = false; // 塗りつぶしモード

    public int undoStackCount { get { return undoStack.Count; } }
    public int redoStackCount { get { return redoStack.Count; } }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Texture2Dを作成
        texture = new Texture2D(CanvasWidth, CanvasHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point; // ドット絵くっきりモードOn

        // スタックの初期生成
        undoStack = new Stack<Color[]>();
        redoStack = new Stack<Color[]>();

        // テクスチャを初期化(白で塗りつぶす)
        Color[] colors = new Color[CanvasWidth * CanvasHeight];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        texture.SetPixels(colors);
        texture.Apply();

        undoStack.Push(texture.GetPixels());
        drawingPanel.texture = texture;
    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingPanel.rectTransform, Input.mousePosition, null, out localPoint);


        if (Input.GetMouseButtonDown(0))
        {
            if (isFillMode)
            {
                FloodFill(localPoint);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (isPenMode) 
            {
                DrawAtPoint(localPoint);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastPoint = null;
            if (IsInsideCanvas(localPoint))
            {
                SaveUndo();
            }
        }
    }

    private void DrawAtPoint(Vector2 localPoint)
    {
        Rect rect = drawingPanel.rectTransform.rect;

        // ローカル座標をTexture2Dの座標に変換
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
        {
            if (lastPoint.HasValue)
            {
                DrawLine(lastPoint.Value.x, lastPoint.Value.y, x, y); // 2回目以降の描画
            }
            else
            {
                DrawPoint(x, y); // 最初の描画
            }

            lastPoint = new Vector2Int(x, y);
            texture.Apply();
        }
    }

    // 最初のクリックの描画
    private void DrawPoint(int cx, int cy)
    {
        /*
        // ペンの透明度を設定
        Color newColor = drawColor;
        newColor.a = penAlpha;
        Color brushColor = Color.Lerp(Color.white, newColor, newColor.a);
        */

        if (brushSize == 1)
        {
            texture.SetPixel(cx, cy, drawColor);
        }
        // ブラシの大きさが偶数の場合と奇数の場合で処理を分ける
        else if (brushSize % 2 == 0)
        {
            int halfSize = brushSize / 2;
            for (int dx = -halfSize + 1; dx <= halfSize; dx++)
            {
                for (int dy = -halfSize + 1; dy <= halfSize; dy++)
                {
                    int px = cx + dx;
                    int py = cy + dy;
                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                    {
                        texture.SetPixel(px, py, drawColor);
                    }
                }
            }
        }
        else if (brushSize % 2 == 1)
        {
            int halfSize = brushSize / 2;
            for (int dx = -halfSize; dx <= halfSize; dx++)
            {
                for (int dy = -halfSize; dy <= halfSize; dy++)
                {
                    int px = cx + dx;
                    int py = cy + dy;
                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                    {
                        texture.SetPixel(px, py, drawColor);
                    }
                }
            }
        }
    }

    // Bresenhamの直線アルゴリズム
    private void DrawLine(int x0, int y0, int x1, int y1)
    {
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
    }

    private void SaveUndo()
    {
        undoStack.Push(texture.GetPixels()); // 現在の状態を保存
        redoStack.Clear(); // 新しく描画したらRedo履歴はクリア
    }

    public void Undo()
    { 
        if (undoStackCount > 1)
        {
            redoStack.Push(undoStack.Pop());
            texture.SetPixels(undoStack.Peek());
            texture.Apply();
        }
        else
        {
            Debug.Log("Cannot undo");
        }
    }
    public void Redo()
    {
        if (redoStackCount > 0)
        {
            undoStack.Push(redoStack.Pop());
            texture.SetPixels(undoStack.Peek());
            texture.Apply();
        }
        else
        {
            Debug.Log("Cannot redo");
        }
    }

    // ドット絵をすべて削除する
    public void ClearCanvas()
    {
        Color[] clearColors = new Color[texture.width * texture.height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.white;
        }
        texture.SetPixels(clearColors);
        texture.Apply();

        SaveUndo();
    }

    private bool IsInsideCanvas(Vector2 localPoint)
    {
        Rect rect = drawingPanel.rectTransform.rect;
        return localPoint.x >= rect.x && localPoint.x <= rect.x + rect.width
            && localPoint.y >= rect.y && localPoint.y <= rect.y + rect.height;
    }

    public void FloodFill(Vector2 localPoint)
    {
        Rect rect = drawingPanel.rectTransform.rect;

        // ローカル座標をTexture2Dの座標に変換
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        Color targetColor = texture.GetPixel(x, y);
        // クリックした場所の色と塗りつぶし色が同じ場合は何もしない
        if (targetColor == drawColor)
        {
            return;
        }

        Queue<Vector2Int> pixelQueue = new Queue<Vector2Int>();
        pixelQueue.Enqueue(new Vector2Int(x, y));

        while (pixelQueue.Count > 0)
        {
            Vector2Int pos = pixelQueue.Dequeue();

            if (pos.x < 0 || pos.x >= texture.width || pos.y < 0 || pos.y >= texture.height)
                continue;

            if (texture.GetPixel(pos.x, pos.y) != targetColor)
                continue;

            texture.SetPixel(pos.x, pos.y, drawColor);

            pixelQueue.Enqueue(new Vector2Int(pos.x + 1, pos.y));
            pixelQueue.Enqueue(new Vector2Int(pos.x - 1, pos.y));
            pixelQueue.Enqueue(new Vector2Int(pos.x, pos.y + 1));
            pixelQueue.Enqueue(new Vector2Int(pos.x, pos.y - 1));
        }
        texture.Apply();
    }
}
