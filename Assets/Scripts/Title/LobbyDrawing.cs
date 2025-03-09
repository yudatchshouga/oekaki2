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
    Dictionary<int, Color> playerColors = new Dictionary<int, Color>(); // �v���C���[���Ƃ̐F�ݒ�
    Dictionary<int, int> playerPenSizes = new Dictionary<int, int>(); // �v���C���[���Ƃ̃y���T�C�Y�ݒ�
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

        if (Input.GetMouseButton(0))
        {
            if (IsInsideCanvas(localPoint))
            {
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                photonView.RPC("DrawAtPoint", RpcTarget.All, actorNumber, localPoint.x, localPoint.y, drawColor.r, drawColor.g, drawColor.b, drawColor.a, penSize);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            photonView.RPC("ResetLastPoint", RpcTarget.All, actorNumber);
        }
    }

    [PunRPC]
    private void DrawAtPoint(int actorNumber, int x, int y, float r, float g, float b, float a, int size)
    {
        Vector2Int point = new Vector2Int(x, y);
        Color color = new Color(r, g, b, a);

        // �v���C���[�̐ݒ���X�V
        playerColors[actorNumber] = color;
        playerPenSizes[actorNumber] = size;

        if (!lastPoints.ContainsKey(actorNumber))
        {
            lastPoints[actorNumber] = null;
        }

        // �ꎞ�I�ɂ��̃v���C���[�ݒ��DrawingUtils���g��
        DrawingUtils tempDrawer = new DrawingUtils(texture, color, size);

        if (lastPoints[actorNumber].HasValue)
        {
            tempDrawer.DrawLine(lastPoints[actorNumber].Value, point); // 2��ڈȍ~�̕`��
        }
        else
        {
            tempDrawer.DrawPoint(point); // �ŏ��̕`��
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

    private bool IsInsideCanvas(Vector2Int localPoint)
    {
        Rect rect = rawImage.rectTransform.rect;
        return rect.Contains(localPoint);
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

    // �p���b�g�̃{�^�����N���b�N�����Ƃ���drawColor��ύX����
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
        drawer = new DrawingUtils(texture, drawColor, penSize);
    }

    // �X���C�_�[�̒l��penSize�ɔ��f����
    public void OnValueChangedPenSize(Slider slider)
    {
        penSize = (int)slider.value;
        drawer = new DrawingUtils(texture, drawColor, penSize);
    }
}
