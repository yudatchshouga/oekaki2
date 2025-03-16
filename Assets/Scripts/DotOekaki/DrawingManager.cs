using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
//using Unity.Mathematics;
using Photon.Pun;
using Photon.Realtime;

public class DrawingManager : MonoBehaviourPunCallbacks
{
    public static DrawingManager instance;

    Texture2D texture;
    [SerializeField] GameObject drawField;
    [SerializeField] RawImage drawingPanel;
    [SerializeField] Text answer;
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
    DrawingUtils drawer;
    public enum ToolMode
    {
        Pen,
        Fill,
        Line,
        Circle,
        Rectrangle,
    }
    public ToolMode currentMode;

    public int undoStackCount { get { return undoStack.Count; } }
    public int redoStackCount { get { return redoStack.Count; } }



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Debug.Log("DrawingManager Start");
        CanvasWidth = PlayerPrefs.GetInt("Width", 50);
        CanvasHeight = PlayerPrefs.GetInt("Height", 50);

        // Texture2Dを作成
        texture = new Texture2D(CanvasWidth, CanvasHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point; // ドット絵くっきりモード

        // スタックの初期生成
        undoStack = new Stack<Color[]>();
        redoStack = new Stack<Color[]>();

        // テクスチャを初期化
        ClearCanvas();

        undoStack.Push(texture.GetPixels());
        drawingPanel.texture = texture;

        // ゲーム開始時はペンモード
        currentMode = ToolMode.Pen;

        drawer = new DrawingUtils(texture, drawColor, brushSize);

        SetDrawFieldSize();

        string role = PlayerPrefs.GetString("role", "answerer");
        Debug.Log(role);
        if (role == "answerer")
        {
            answer.text = "お題はなんでしょう？";
        }
        else
        {
            answer.text = "お題：ヨクバリス";
        }

        // photonの接続状態を確認
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Photonに接続済み");
        }
        else
        {
            Debug.Log("Photonに未接続");
        }
    }

    // 描画領域のサイズを設定
    private void SetDrawFieldSize()
    {
        RectTransform rectTransform = drawField.GetComponent<RectTransform>();
        float aspectRatio = (float)CanvasWidth / CanvasHeight;

        if (aspectRatio > 1)
        {
            rectTransform.sizeDelta = new Vector2(900, 900 / aspectRatio);
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(900 * aspectRatio, 900);
        }
    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingPanel.rectTransform, Input.mousePosition, null, out localPoint);

        Rect rect = drawingPanel.rectTransform.rect;
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        if (Input.GetMouseButtonDown(0))
        {
            if (currentMode == ToolMode.Pen)
            { 
                if (IsInsideCanvas(localPoint))
                {
                    isDrawing = true;
                }
            }

            // 塗りつぶしツール
            if (currentMode == ToolMode.Fill)
            {
                FloodFill(localPoint);
            }

            // 円、長方形ツールの始点の設定
            if (currentMode == ToolMode.Circle || currentMode == ToolMode.Rectrangle)
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

        if (Input.GetMouseButton(0))
        {
            if (currentMode == ToolMode.Pen) 
            {
                DrawAtPoint(localPoint);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastPoint = null;
            if (currentMode == ToolMode.Pen)
            {
                if (isDrawing)
                {
                    SaveUndo();
                    isDrawing = false;
                }
            }

            // 直線ツール
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
                    startPoint = null;
                    SaveUndo();
                }
            }

            // 円、長方形ツール
            if (currentMode == ToolMode.Circle || currentMode == ToolMode.Rectrangle)
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
        else if (currentMode == ToolMode.Rectrangle)
        {
            drawer.DrawRectangle(start, end);
        }
        texture.Apply();
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

    private bool IsInsideCanvas(Vector2 localPoint)
    {
        Rect rect = drawingPanel.rectTransform.rect;
        return localPoint.x >= rect.x && localPoint.x <= rect.x + rect.width
            && localPoint.y >= rect.y && localPoint.y <= rect.y + rect.height;
    }

    // 塗りつぶし
    private void FloodFill(Vector2 localPoint)
    {
        // キャンバス外をクリックした場合は何もしない
        if (!IsInsideCanvas(localPoint))
        {
            return;
        }

        drawer.FloddFill(drawingPanel, localPoint);
        texture.Apply();
        SaveUndo();
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

    // テキストをセット
    public void SetAnswerText(string text)
    {
        answer.text = text;
    }
}
