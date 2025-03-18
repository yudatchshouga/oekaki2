using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HueSlider : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    RawImage hueRawImage; // HueスライダーのRawImage
    RectTransform hueRawImageRect; // HueスライダーのRectTransform
    Texture2D hueTexture;
    [SerializeField] Image hueCursorImage; // Hueスライダーのカーソル
    [SerializeField] RectTransform hueCursor; // Hueスライダーのカーソル
    [SerializeField] SVMap svMap; // SVマップ

    private float hue;

    void Start()
    {
        hueRawImage = GetComponent<RawImage>();
        hueRawImageRect = hueRawImage.GetComponent<RectTransform>();

        CreateHueTexture();
        hueRawImage.texture = hueTexture;

        // 初期値を設定
        hue = 0f;
        hueCursorImage.color = Color.red;
    }

    // Hueスライダー用のTexture2Dを作成
    private void CreateHueTexture()
    {
        int width = 256;  // スライダーの幅
        int height = 1;   // 高さは1ピクセル

        hueTexture = new Texture2D(width, height);

        // 色相0〜1のグラデーションを生成
        for (int x = 0; x < width; x++)
        {
            float hue = (float)x / (width - 1);  // 横位置に基づいて色相を設定
            Color color = Color.HSVToRGB(hue, 1f, 1f);  // 彩度と明度を最大に設定
            hueTexture.SetPixel(x, 0, color);
        }

        hueTexture.Apply();  // 変更を適用
    }

    // クリック時の処理
    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateHue(eventData);
        svMap.UpdateSpectrumTexture(hue);
        svMap.UpdateCursorColor(Color.HSVToRGB(hue, svMap.s, svMap.v));
    }

    // ドラッグ時の処理
    public void OnDrag(PointerEventData eventData)
    {
        UpdateHue(eventData);
        svMap.UpdateSpectrumTexture(hue);
        svMap.UpdateCursorColor(Color.HSVToRGB(hue, svMap.s, svMap.v));
    }

    // H値を更新する共通処理
    private void UpdateHue(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            hueRawImageRect, eventData.position, eventData.pressEventCamera, out localPoint);

        // スライダーの範囲に収める
        Rect rect = hueRawImageRect.rect;
        localPoint.x = Mathf.Clamp(localPoint.x, rect.xMin, rect.xMax);

        // 色相を更新
        hue = Mathf.InverseLerp(rect.xMin, rect.xMax, localPoint.x);

        // 選択された色を表示
        Color selectedColor = Color.HSVToRGB(hue, 1f, 1f);
        hueCursorImage.color = selectedColor;

        // カーソルの位置を更新
        hueCursor.anchoredPosition = new Vector2(localPoint.x, 0);
    }
}
