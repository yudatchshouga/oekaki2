using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class DrawingManager : MonoBehaviourPunCallbacks
{
    public static DrawingManager instance;

    Texture2D texture;
    [SerializeField] GameObject drawField;
    [SerializeField] RawImage drawingPanel;
    [SerializeField] Text roleText;
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
    public bool isDrawable; // 描画可能かどうか
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
    public Role role;

    public int undoStackCount { get { return undoStack.Count; } }
    public int redoStackCount { get { return redoStack.Count; } }

    Dictionary<int, Vector2Int?> lastPoints = new Dictionary<int, Vector2Int?>();
    Dictionary<int, Color> playerColors = new Dictionary<int, Color>(); // プレイヤーごとの色設定
    Dictionary<int, int> playerPenSizes = new Dictionary<int, int>(); // プレイヤーごとのペンサイズ設定


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
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

        // オンラインモードとオフラインモードで処理を分ける
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("オンラインモードで実行");
            photonView.RPC("SetDrawFieldSize", RpcTarget.All);

            role = PlayerPrefs.GetString("role").toRole();
            Debug.Log(role);
            if (role == Role.Questioner)
            {
                isDrawable = true;
                roleText.text = "あなたは描き手です";
            }
            else
            {
                isDrawable = false;
                roleText.text = "あなたは回答者です";
            }
        }
        else
        {
            Debug.Log("オフラインモードで実行");
            SetDrawFieldSize();
        }
    }

    // 描画領域のサイズを設定
    [PunRPC]
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
                    if (PhotonNetwork.InRoom)
                    {
                        if (IsInsideCanvas(localPoint))
                        {
                            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                            photonView.RPC("DrawAtPointRPC", RpcTarget.All, actorNumber, x, y, drawColor.r, drawColor.g, drawColor.b, drawColor.a, brushSize);
                        }
                    }
                    else
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
                    if (PhotonNetwork.InRoom)
                    {
                        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                        photonView.RPC("ResetLastPoint", RpcTarget.All, actorNumber);
                        photonView.RPC("SaveUndo", RpcTarget.All);
                    }
                    else
                    {
                        SaveUndo();
                    }
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
                    if (PhotonNetwork.InRoom)
                    {
                        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                        photonView.RPC("FloodFillRPC", RpcTarget.All, actorNumber, localPoint, drawColor.r, drawColor.g, drawColor.b, drawColor.a, brushSize);
                    }
                    else
                    {
                        FloodFill(localPoint);
                    }
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
                        if (PhotonNetwork.InRoom)
                        {
                            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                            photonView.RPC("DrawShapeRPC", RpcTarget.All, actorNumber, startPoint.Value.x, startPoint.Value.y, pixelPos.x, pixelPos.y, drawColor.r, drawColor.g, drawColor.b, drawColor.a, brushSize);
                            photonView.RPC("SaveUndo", RpcTarget.All);
                        }
                        else
                        {
                            DrawShape(startPoint.Value, pixelPos);
                            SaveUndo();
                        }
                        startPoint = null;
                    }
                }
            }
        }

        // 円、長方形ツール
        else if (currentMode == ToolMode.Circle || currentMode == ToolMode.Rectrangle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 始点の設定
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
            if (Input.GetMouseButtonUp(0))
            {
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
                        if (PhotonNetwork.InRoom)
                        {
                            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                            photonView.RPC("DrawShapeRPC", RpcTarget.All, actorNumber, startPixel.x, startPixel.y, endPixel.x, endPixel.y, drawColor.r, drawColor.g, drawColor.b, drawColor.a, brushSize);
                            photonView.RPC("SaveUndo", RpcTarget.All);
                        }
                        else
                        {
                            DrawShape(startPixel, endPixel);
                            SaveUndo();
                        }
                        isDrawing = false;
                    }
                }
            }
        }
    }

    [PunRPC]
    private void DrawAtPointRPC(int actorNumber, int x, int y, float r, float g, float b, float a, int size)
    {
        Vector2Int point = new Vector2Int(x, y);
        Color color = new Color(r, g, b, a);

        // プレイヤーの設定を更新
        playerColors[actorNumber] = color;
        playerPenSizes[actorNumber] = size;

        if (!lastPoints.ContainsKey(actorNumber))
        {
            lastPoints[actorNumber] = null;
        }

        // 一時的にそのプレイヤー設定でDrawingUtilsを使う
        DrawingUtils tempDrawer = new DrawingUtils(texture, color, size);

        if (IsInsideCanvas(new Vector2(x, y)))
        {
            if (lastPoints[actorNumber].HasValue)
            {
                tempDrawer.DrawLine(lastPoints[actorNumber].Value, point); // 2回目以降の描画
            }
            else
            {
                tempDrawer.DrawPoint(point); // 最初の描画
            }
            lastPoints[actorNumber] = point;
        }
        texture.Apply();
    }

    [PunRPC]
    private void ResetLastPoint(int actorNumber)
    {
        if (lastPoints.ContainsKey(actorNumber))
        {
            lastPoints[actorNumber] = null;
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

    [PunRPC]
    private void DrawShapeRPC(int actorNumber, int startX, int startY, int endX, int endY, float r, float g, float b, float a, int size)
    {
        Color color = new Color(r, g, b, a);

        // プレイヤーの設定を更新
        playerColors[actorNumber] = color;
        playerPenSizes[actorNumber] = size;

        // 一時的にそのプレイヤー設定でDrawingUtilsを使う
        DrawingUtils tempDrawer = new DrawingUtils(texture, color, size);

        if (currentMode == ToolMode.Line)
        {
            tempDrawer.DrawLine(new Vector2Int(startX, startY), new Vector2Int(endX, endY));
        }
        else if (currentMode == ToolMode.Circle)
        {
            tempDrawer.DrawCircle(new Vector2Int(startX, startY), new Vector2Int(endX, endY));
        }
        else if (currentMode == ToolMode.Rectrangle)
        {
            tempDrawer.DrawRectangle(new Vector2Int(startX, startY), new Vector2Int(endX, endY));
        }
        texture.Apply();
    }

    private void FloodFill(Vector2 localPoint)
    {
        drawer.FloodFill(drawingPanel, localPoint);
        texture.Apply();
        SaveUndo();
    }

    [PunRPC]
    private void FloodFillRPC(int actorNumber, Vector2 localPoint, float r, float g, float b, float a, int size)
    {
        Color color = new Color(r, g, b, a);

        // プレイヤーの設定を更新
        playerColors[actorNumber] = color;
        playerPenSizes[actorNumber] = size;

        // 一時的にそのプレイヤー設定でDrawingUtilsを使う
        DrawingUtils tempDrawer = new DrawingUtils(texture, color, size);

        tempDrawer.FloodFill(drawingPanel, localPoint);
        texture.Apply();
        SaveUndo();
    }

    [PunRPC]
    private void SaveUndo()
    {
        undoStack.Push(texture.GetPixels()); // 現在の状態を保存
        redoStack.Clear(); // 新しく描画したらRedo履歴はクリア
    }

    public void UndoButton()
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("Undo", RpcTarget.All);
        }
        else
        {
            Undo();
        }
    }
    public void RedoButton() 
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("Redo", RpcTarget.All);
        }
        else
        {
            Redo();
        }
    }

    [PunRPC]
    private void Undo()
    { 
        if (undoStackCount > 1)
        {
            redoStack.Push(undoStack.Pop());
            texture.SetPixels(undoStack.Peek());
            texture.Apply();
        }
    }
    [PunRPC]
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
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("ClearCanvas", RpcTarget.All);
            photonView.RPC("SaveUndo", RpcTarget.All);
        }
        else
        {
            ClearCanvas();
            SaveUndo();
        }
    }

    [PunRPC]
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
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("ChangeModeRPC", RpcTarget.All, mode);
        }
        else
        {
            currentMode = mode;
        }
        isDrawing = false;
    }

    [PunRPC]
    private void ChangeModeRPC(ToolMode mode)
    {
        currentMode = mode;
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
