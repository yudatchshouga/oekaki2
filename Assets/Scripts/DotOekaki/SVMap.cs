using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SVMap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] RawImage svRawImage; // SVマップのRawImage
    [SerializeField] RectTransform svCursor; // SV選択カーソル
    [SerializeField] Image svCursorImage;  // 選択した色の表示

    Texture2D spectrumTexture;
    RectTransform svRawImageRect;
    Color selectedColor;

    public float s;
    public float v;

    void Start()
    {
        // RawImageのRectTransformを取得
        svRawImageRect = svRawImage.GetComponent<RectTransform>();

        // カラースペクトラム用のTexture2Dを作成
        CreateSpectrumTexture(0f);
        svRawImage.texture = spectrumTexture;

        // 初期値を設定
        s = 1f;
        v = 1f;
        svCursorImage.color = Color.red;
    }

    // スペクトラム用のTexture2Dを作成
    private void CreateSpectrumTexture(float hue)
    {
        int width = 256;  // 彩度 (S) の範囲（横軸）
        int height = 256; // 明度 (V) の範囲（縦軸）

        spectrumTexture = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 明度（V）と彩度（S）の値を正規化
                float s = (float)x / width;
                float v = (float)y / height;

                // 色相は常に一定
                Color color = HsvToRgb(hue, s, v);

                // ピクセルを設定
                spectrumTexture.SetPixel(x, y, color);
            }
        }

        spectrumTexture.Apply();  // 変更を適用
    }

    public void UpdateSpectrumTexture(float hue)
    {
        CreateSpectrumTexture(hue);
        svRawImage.texture = spectrumTexture;
    }

    // HSV → RGB 変換
    private Color HsvToRgb(float h, float s, float v)
    {
        return Color.HSVToRGB(h, s, v);
    }

    // カラースペクトラムをクリックした時に色を選択
    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateSelectedColor(eventData);
        DrawingManager.instance.ChangeColor(selectedColor);
    }

    // カラースペクトラムをドラッグした時に色を選択
    public void OnDrag(PointerEventData eventData)
    {
        UpdateSelectedColor(eventData);
        DrawingManager.instance.ChangeColor(selectedColor);
    }

    // クリック位置に基づいて色を更新
    public void UpdateSelectedColor(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            svRawImageRect, eventData.position, eventData.pressEventCamera, out localPoint);

        // SVマップの範囲に収める
        Rect rect = svRawImageRect.rect;
        localPoint.x = Mathf.Clamp(localPoint.x, rect.xMin, rect.xMax);
        localPoint.y = Mathf.Clamp(localPoint.y, rect.yMin, rect.yMax);

        // 色を選択
        s = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x); // 彩度
        v = Mathf.InverseLerp(rect.yMin, rect.yMax, localPoint.y); // 明度

        // テクスチャの対応するピクセルを取得
        selectedColor = spectrumTexture.GetPixel(
            Mathf.FloorToInt(s * (spectrumTexture.width - 1)),
            Mathf.FloorToInt(v * (spectrumTexture.height - 1))
        );

        // 選択した色を表示
        svCursorImage.color = selectedColor;

        svCursor.anchoredPosition = localPoint;
    }

    // カーソルの色を更新
    public void UpdateCursorColor(Color color)
    {
        svCursorImage.color = color;
    }
}
