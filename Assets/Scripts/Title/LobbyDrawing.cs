using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyDrawing : MonoBehaviourPunCallbacks
{
    Texture2D texture;
    [SerializeField] RawImage rawImage;
    Color drawColor;
    int penSize;
    int CanvasWidth;
    int CanvasHeight;
    Vector2Int? lastPoint = null;
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

        ClearCanvas();
    }

    void Update()
    {
        drawer = new DrawingUtils(texture, drawColor, penSize);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImage.rectTransform, Input.mousePosition, null, out localPoint);

        Rect rect = rawImage.rectTransform.rect;
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        if (Input.GetMouseButton(0))
        {
            if (IsInsideCanvas(localPoint))
            {
                photonView.RPC("DrawAtPoint", RpcTarget.All, localPoint);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastPoint = null;
        }
    }

    [PunRPC]
    private void DrawAtPoint(Vector2 localPoint)
    {
        Rect rect = rawImage.rectTransform.rect;

        // ローカル座標をTexture2Dの座標に変換
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        if (lastPoint.HasValue)
        {
            drawer.DrawLine(lastPoint.Value, new Vector2Int(x, y)); // 2回目以降の描画
        }
        else
        {
            drawer.DrawPoint(new Vector2Int(x, y)); // 最初の描画
        }

        lastPoint = new Vector2Int(x, y);
        texture.Apply();
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

    private bool IsInsideCanvas(Vector2 localPoint)
    {
        Rect rect = rawImage.rectTransform.rect;
        return rect.Contains(localPoint);
    }

    // パレットのボタンをクリックしたときにdrawColorを変更する
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
                drawColor = new Color32(246, 184, 148, 1);
                break;
        }
    }

    // スライダーの値をpenSizeに反映する
    public void OnValueChangedPenSize(Slider slider)
    {
        penSize = (int)slider.value;
    }
}
