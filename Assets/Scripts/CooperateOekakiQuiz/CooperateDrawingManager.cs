using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class CooperateDrawingManager : MonoBehaviourPunCallbacks
{
    public static CooperateDrawingManager instance;

    public Texture2D texture;
    [SerializeField] GameObject drawField;
    [SerializeField] RawImage drawingPanel;
    public int CanvasWidth; // キャンバスの横幅
    public int CanvasHeight; // キャンバスの縦幅
    public Color drawColor; // ペンの色
    public int brushSize; // ブラシの大きさ
    Vector2Int? lastPoint = null; // 前回の描画位置
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

    Dictionary<int, Vector2Int?> lastPoints = new Dictionary<int, Vector2Int?>();
    Dictionary<int, Color> playerColors = new Dictionary<int, Color>(); // プレイヤーごとの色設定
    Dictionary<int, int> playerPenSizes = new Dictionary<int, int>(); // プレイヤーごとのペンサイズ設定

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
    [PunRPC]
    private void CreateTexture(int width, int height)
    {
        CanvasWidth = width;
        CanvasHeight = height;
        texture = new Texture2D(CanvasWidth, CanvasHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point; // ドット絵くっきりモード
        ClearCanvas(); // テクスチャを初期化
        drawingPanel.texture = texture;
        drawer = new DrawingUtils(texture, drawColor, brushSize);
    }

    // 描画領域のサイズを設定
    [PunRPC]
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
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("CreateTexture", RpcTarget.All, width, height);
            photonView.RPC("SetDrawFieldSize", RpcTarget.All, width, height);
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
                        }
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
                        if (PhotonNetwork.InRoom)
                        {
                            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                            photonView.RPC("DrawShapeRPC", RpcTarget.All, actorNumber, startPixel.x, startPixel.y, endPixel.x, endPixel.y, drawColor.r, drawColor.g, drawColor.b, drawColor.a, brushSize);
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
        else if (currentMode == ToolMode.Rectangle)
        {
            tempDrawer.DrawRectangle(new Vector2Int(startX, startY), new Vector2Int(endX, endY));
        }
        texture.Apply();
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
            ChangeModeRPC(mode);
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
