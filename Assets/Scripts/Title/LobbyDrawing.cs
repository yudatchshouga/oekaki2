using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;

public class LobbyDrawing : MonoBehaviourPunCallbacks
{
    Texture2D texture;
    [SerializeField] RawImage rawImage;
    Color drawColor;
    int penSize;
    int CanvasWidth;
    int CanvasHeight;
    Dictionary<int, Vector2Int?> lastPoints = new Dictionary<int, Vector2Int?>();
    Dictionary<int, Color> playerColors = new Dictionary<int, Color>(); // 繝励Ξ繧､繝､繝ｼ縺斐→縺ｮ濶ｲ險ｭ螳・
    Dictionary<int, int> playerPenSizes = new Dictionary<int, int>(); // 繝励Ξ繧､繝､繝ｼ縺斐→縺ｮ繝壹Φ繧ｵ繧､繧ｺ險ｭ螳・
    DrawingUtils drawer;

    private void Start()
    {
        CanvasWidth = 96;
        CanvasHeight = 54;
        texture = new Texture2D(CanvasWidth, CanvasHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        rawImage.texture = texture;
        drawColor = Color.black;
        penSize = 1;
        drawer = new DrawingUtils(texture, drawColor, penSize);

        ClearCanvas();
    }

    void Update()
    {
        Vector2Int localPoint = GetMouseCanvasPosition();

        if (IsInsideCanvas(localPoint))
        {
            if (Input.GetMouseButton(0))
            {
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                photonView.RPC("DrawAtPoint", RpcTarget.All, actorNumber, localPoint.x, localPoint.y, drawColor.r, drawColor.g, drawColor.b, drawColor.a, penSize);
            }

            if (Input.GetMouseButtonUp(0))
            {
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                photonView.RPC("ResetLastPoint", RpcTarget.All, actorNumber);
            }
        }
    }

    [PunRPC]
    private void DrawAtPoint(int actorNumber, int x, int y, float r, float g, float b, float a, int size)
    {
        Vector2Int point = new Vector2Int(x, y);
        Color color = new Color(r, g, b, a);

        // 繝励Ξ繧､繝､繝ｼ縺ｮ險ｭ螳壹ｒ譖ｴ譁ｰ
        playerColors[actorNumber] = color;
        playerPenSizes[actorNumber] = size;

        if (!lastPoints.ContainsKey(actorNumber))
        {
            lastPoints[actorNumber] = null;
        }

        // 荳譎ら噪縺ｫ縺昴・繝励Ξ繧､繝､繝ｼ險ｭ螳壹〒DrawingUtils繧剃ｽｿ縺・
        DrawingUtils tempDrawer = new DrawingUtils(texture, color, size);

        if (lastPoints[actorNumber].HasValue)
        {
            tempDrawer.DrawLine(lastPoints[actorNumber].Value, point); // 2蝗樒岼莉･髯阪・謠冗判
        }
        else
        {
            tempDrawer.DrawPoint(point); // 譛蛻昴・謠冗判
        }
        texture.Apply();
        lastPoints[actorNumber] = point;
    }

    [PunRPC]
    private void ResetLastPoint(int actorNumber)
    {
        if (lastPoints.ContainsKey(actorNumber))
        {
            lastPoints[actorNumber] = null;
        }
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

    // キャンバス内かどうかを判定する
    private bool IsInsideCanvas(Vector2Int localPoint)
    {
        return localPoint.x >= 0 && localPoint.x < CanvasWidth && localPoint.y >= 0 && localPoint.y < CanvasHeight;
    }

    private Vector2Int GetMouseCanvasPosition()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImage.rectTransform, Input.mousePosition, null, out localPoint);

        Rect rect = rawImage.rectTransform.rect;
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);
        return new Vector2Int(x, y);
    }

    // 繝代Ξ繝・ヨ縺ｮ繝懊ち繝ｳ繧偵け繝ｪ繝・け縺励◆縺ｨ縺阪↓drawColor繧貞､画峩縺吶ｋ
    public void OnClickColorButton(int index)
    {
        switch (index)
        {
            case 0:
                drawColor = Color.black;
                break;
            case 1:
                drawColor = Color.white;
                break;
            case 2:
                drawColor = Color.red;
                break;
            case 3:
                drawColor = Color.green;
                break;
            case 4:
                drawColor = Color.blue;
                break;
            case 5:
                drawColor = Color.yellow;
                break;
            case 6:
                drawColor = Color.magenta;
                break;
            case 7:
                drawColor = Color.cyan;
                break;
            case 8:
                drawColor = Color.gray;
                break;
            case 9:
                drawColor = new Color32(246, 184, 148, 255);
                break;
        }
        drawer = new DrawingUtils(texture, drawColor, penSize);
    }

    // 繧ｹ繝ｩ繧､繝繝ｼ縺ｮ蛟､繧恥enSize縺ｫ蜿肴丐縺吶ｋ
    public void OnValueChangedPenSize(Slider slider)
    {
        penSize = (int)slider.value;
        drawer = new DrawingUtils(texture, drawColor, penSize);
    }
}
