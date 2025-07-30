using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DengonDrawingManager : MonoBehaviour
{
    public static DengonDrawingManager instance;

    public Texture2D texture;
    [SerializeField] GameObject drawField;
    [SerializeField] RawImage drawingPanel;
    public int CanvasWidth; // キャンバスの横幅
    public int CanvasHeight; // キャンバスの縦幅
    public Color drawColor; // ペンの色
    public int brushSize; // ブラシの大きさ
    Vector2Int? lastPoint = null; // 前回の描画位置
    Stack<Color[]> undoStack; // 元に戻すためのスタック
    Stack<Color[]> redoStack; // やり直しのためのスタック
    Vector2Int? startPoint = null; // 直線モードの始点
    Vector2Int startPixel; // 円モード、長方形モードの始点
    bool isDrawing = false; // 描画中かどうか
    public bool isDrawable = false; // 描画可能かどうか
    DrawingUtils drawer;

    public enum ToolMode
    {
        Pen,
        Fill,
        Line,
        Circle,
        Rectangle,
    }
    public ToolMode currentMode;

    public int undoStackCount { get { return undoStack.Count; } }
    public int redoStackCount { get { return redoStack.Count; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeDrawField();
    }

    public void InitializeDrawField()
    {
        // キャンバスの初期サイズ
        CanvasWidth = CanvasHeight = 50; // 初期サイズを設定

        // スタックの初期生成
        undoStack = new Stack<Color[]>();
        redoStack = new Stack<Color[]>();

        // Texture2Dを作成
        CreateTexture(CanvasWidth, CanvasHeight);

        // ゲーム開始時はペンモード
        currentMode = ToolMode.Pen;

        SetDrawFieldSize(CanvasWidth, CanvasHeight);

        // 初期の描画色とブラシサイズ
        drawColor = Color.black; // 初期の描画色
        brushSize = 1; // 初期のブラシサイズ
    }

    // テクスチャを作成
    private void CreateTexture(int width, int height)
    {
        CanvasWidth = width;
        CanvasHeight = height;
        texture = new Texture2D(CanvasWidth, CanvasHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point; // ドット絵くっきりモード
        ClearCanvas(); // テクスチャを初期化
        drawingPanel.texture = texture;
        undoStack.Clear();
        redoStack.Clear();
        undoStack.Push(texture.GetPixels());
        drawer = new DrawingUtils(texture, drawColor, brushSize);
    }

    // 描画領域のサイズを設定
    private void SetDrawFieldSize(int width, int height)
    {
        RectTransform rectTransform = drawField.GetComponent<RectTransform>();
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

    public void ResetDrawFieldSize(int width, int height)
    {
        CreateTexture(width, height);
        SetDrawFieldSize(width, height);
    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingPanel.rectTransform, Input.mousePosition, null, out localPoint);

        Rect rect = drawingPanel.rectTransform.rect;
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        if (!isDrawable)
        {
            return;
        }

        // ペンツール
        if (currentMode == ToolMode.Pen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsInsideCanvas(localPoint))
                {
                    isDrawing = true;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (isDrawing)
                {
                    if (IsInsideCanvas(localPoint))
                    {
                        DrawAtPoint(localPoint);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                lastPoint = null;
                if (isDrawing)
                {
                    SaveUndo();
                    isDrawing = false;
                }
            }
        }

        // 塗りつぶしツール
        else if (currentMode == ToolMode.Fill)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentMode == ToolMode.Fill)
                {
                    if (!IsInsideCanvas(localPoint))
                    {
                        return;
                    }
                    FloodFill(localPoint);
                }
            }
        }

        // 直線ツール
        else if (currentMode == ToolMode.Line)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (currentMode == ToolMode.Line)
                {
                    if (!IsInsideCanvas(localPoint))
                    {
                        return;
                    }
                    Vector2Int pixelPos = new Vector2Int(x, y);
                    // 一回目のクリックで始点を設定
                    if (startPoint == null)
                    {
                        startPoint = pixelPos;
                    }
                    else
                    {
                        DrawShape(startPoint.Value, pixelPos);
                        SaveUndo();
                        startPoint = null;
                    }
                }
            }
        }

        // 円、長方形ツール
        else if (currentMode == ToolMode.Circle || currentMode == ToolMode.Rectangle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 始点の設定
                if (currentMode == ToolMode.Circle || currentMode == ToolMode.Rectangle)
                {
                    if (!IsInsideCanvas(localPoint))
                    {
                        return;
                    }
                    Vector2Int pixelPos = new Vector2Int(x, y);
                    if (!isDrawing)
                    {
                        startPixel = pixelPos;
                        isDrawing = true;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (currentMode == ToolMode.Circle || currentMode == ToolMode.Rectangle)
                {
                    if (isDrawing)
                    {
                        if (!IsInsideCanvas(localPoint))
                        {
                            isDrawing = false;
                            return;
                        }
                        Vector2Int endPixel = new Vector2Int(x, y);
                        DrawShape(startPixel, endPixel);
                        SaveUndo();
                        isDrawing = false;
                    }
                }
            }
        }
    }

    private void DrawAtPoint(Vector2 localPoint)
    {
        Rect rect = drawingPanel.rectTransform.rect;

        // ローカル座標をTexture2Dの座標に変換
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        if (IsInsideCanvas(localPoint))
        {
            if (lastPoint.HasValue)
            {
                drawer.DrawLine(lastPoint.Value, new Vector2Int(x, y)); // 2回目以降の描画
            }
            else
            {
                drawer.DrawPoint(new Vector2Int(x, y)); // 最初の描画
            }

            lastPoint = new Vector2Int(x, y);
        }
        texture.Apply();
    }

    private void DrawShape(Vector2Int start, Vector2Int end)
    {
        if (currentMode == ToolMode.Line)
        {
            drawer.DrawLine(start, end);
        }
        else if (currentMode == ToolMode.Circle)
        {
            drawer.DrawCircle(start, end);
        }
        else if (currentMode == ToolMode.Rectangle)
        {
            drawer.DrawRectangle(start, end);
        }
        texture.Apply();
    }

    private void FloodFill(Vector2 localPoint)
    {
        drawer.FloodFill(drawingPanel, localPoint);
        texture.Apply();
        SaveUndo();
    }

    private void SaveUndo()
    {
        undoStack.Push(texture.GetPixels()); // 現在の状態を保存
        redoStack.Clear(); // 新しく描画したらRedo履歴はクリア
    }

    public void UndoButton()
    {
        Undo();
    }
    public void RedoButton()
    {
        Redo();
    }

    private void Undo()
    {
        if (undoStackCount > 1)
        {
            redoStack.Push(undoStack.Pop());
            texture.SetPixels(undoStack.Peek());
            texture.Apply();
        }
    }
    private void Redo()
    {
        if (redoStackCount > 0)
        {
            undoStack.Push(redoStack.Pop());
            texture.SetPixels(undoStack.Peek());
            texture.Apply();
        }
    }

    public void AllClear()
    {
        ClearCanvas();
        SaveUndo();
    }

    private void ClearCanvas()
    {
        Color[] clearColors = new Color[texture.width * texture.height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = new Color(1, 1, 1, 0);
        }
        texture.SetPixels(clearColors);
        texture.Apply();
    }

    // 盤面に何か描かれているかどうか
    public bool HasDrawing()
    {
        Color32[] pixels = texture.GetPixels32();
        foreach (Color32 pixel in pixels)
        {
            if (pixel.a != 0)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsInsideCanvas(Vector2 localPoint)
    {
        Rect rect = drawingPanel.rectTransform.rect;
        return localPoint.x >= rect.x && localPoint.x <= rect.x + rect.width
            && localPoint.y >= rect.y && localPoint.y <= rect.y + rect.height;
    }

    public void ChangeMode(ToolMode mode)
    {
        currentMode = mode;
        isDrawing = false;
    }

    public void ChangeColor(Color color)
    {
        drawColor = color;
        drawer = new DrawingUtils(texture, drawColor, brushSize);
    }

    public void OnValueChangedBrushSize(Slider slider)
    {
        brushSize = (int)slider.value;
        drawer = new DrawingUtils(texture, drawColor, brushSize);
    }
}
