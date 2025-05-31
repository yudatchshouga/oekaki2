using UnityEngine;
using UnityEngine.UI;

public class EshiritoriImageView : MonoBehaviour
{
    // パネルを宣言
    public GameObject imagePanel;

    void Start()
    {
        string path = "images/Spoit";
        for (int i = 0; i < 5; i++)
        {
            Texture2D texture = Resources.Load<Texture2D>(path);
            if (texture != null)
            {
                GameObject rawImageObject = new GameObject("RawImage");
                rawImageObject.transform.SetParent(imagePanel.transform);

                // RectTransformを設定
                RectTransform rectTransform = rawImageObject.AddComponent<RectTransform>();
                rectTransform.localScale = Vector3.one;

                // RawImageコンポーネントを追加して画像を設定
                RawImage rawImage = rawImageObject.AddComponent<RawImage>();
                rawImage.texture = texture;
            }
            else
            {
                Debug.LogError($"画像 '{path}' が見つかりません。");
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
