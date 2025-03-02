using UnityEngine;

public class ScreenColorPicker : MonoBehaviour
{
    public Camera targetCamera; // 色を取得するためのターゲットカメラ
    private Texture2D screenTexture;

    void Start()
    {
        screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            CaptureScreenAndGetColor(mousePosition);
        }
    }

    private void CaptureScreenAndGetColor(Vector2 screenPosition)
    {
        // カメラからスクリーンショットをキャプチャ
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        targetCamera.targetTexture = renderTexture;
        targetCamera.Render();

        // スクリーンショットをTexture2Dにコピー
        RenderTexture.active = renderTexture;
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();

        // テクスチャのピクセルカラーを取得
        Vector2Int pixelPosition = new Vector2Int((int)screenPosition.x, (int)screenPosition.y);
        Color pixelColor = screenTexture.GetPixel(pixelPosition.x, pixelPosition.y);
        Debug.Log("次の色をクリックしたよ: " + pixelColor);

        // リソースを解放
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
    }
}
