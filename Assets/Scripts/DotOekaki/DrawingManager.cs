using UnityEngine;
using UnityEngine.UI;

public class DrawingManager : MonoBehaviour
{
    public RawImage rawImage;
    private Texture2D texture;
    [SerializeField] Color drawColor = Color.black; // �y���̐F
    [SerializeField] int brushSize = 1; // �u���V�̑傫��

    private void Start()
    {
        int width = 64;
        int height = 64;

        // Texture2D���쐬���A���œh��Ԃ�
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        Color[] clearColors = new Color[width * height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.white;
        }
        texture.SetPixels(clearColors);
        texture.Apply();

        rawImage.texture = texture;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)) // ���N���b�N�܂��̓^�b�`�ŕ`��
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rawImage.rectTransform,
                Input.mousePosition,
                null,
                out localPoint
            );

            DrawAtPoint(localPoint);
        }
    }

    private void DrawAtPoint(Vector2 localPoint)
    {
        Rect rect = rawImage.rectTransform.rect;

        // ���[�J�����W��Texture2D�̍��W�ɕϊ�
        int x = Mathf.FloorToInt((localPoint.x - rect.x) / rect.width * texture.width);
        int y = Mathf.FloorToInt((localPoint.y - rect.y) / rect.height * texture.height);

        if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
        {
            texture.SetPixel(x, y, drawColor);
            texture.Apply();
        }
    }
    /*
    void Start()
    {
        // �e�N�X�`���̍쐬
        texture = rawImage.texture as Texture2D;
        clearPixels = new Color[texture.width * texture.height];

        for (int i = 0; i < clearPixels.Length; i++)
        {
            clearPixels[i] = clearColor;
        }

        ClearCanvas();

        // �e�N�X�`����RawImage�ɐݒ�
        rawImage.texture = texture;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 pixelPos = GetMousePixelPosition();
            Draw(pixelPos);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearCanvas();
        }
    }

    Vector2 GetMousePixelPosition()
    {
        Vector2 localMousePosition = rawImage.rectTransform.InverseTransformPoint(Input.mousePosition);
        Vector2 pixelPos = new Vector2(localMousePosition.x + rawImage.rectTransform.rect.width / 2,
                                       localMousePosition.y + rawImage.rectTransform.rect.height / 2);
        return pixelPos;
    }

    void Draw(Vector2 pixelPos)
    {
        for (int x = -brushSize; x <= brushSize; x++)
        {
            for (int y = -brushSize; y <= brushSize; y++)
            {
                int px = Mathf.Clamp((int)pixelPos.x + x, 0, texture.width - 1);
                int py = Mathf.Clamp((int)pixelPos.y + y, 0, texture.height - 1);
                texture.SetPixel(px, py, drawColor);
            }
        }
        texture.Apply();
    }

    void ClearCanvas()
    {
        texture.SetPixels(clearPixels);
        texture.Apply();
    }
    */
}
