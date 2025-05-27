using UnityEngine;
using UnityEngine.UI;

public class PicturePrefab : MonoBehaviour
{
    [SerializeField] RawImage pictureImage; // 画像を表示するRawImageコンポーネント

    public void SetPicture(Texture2D texture)
    {
        Texture2D savedTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
    }
}
