using UnityEngine;
using UnityEngine.UI;

public class ImageView : MonoBehaviour
{
    [SerializeField] RawImage image;
    [SerializeField] Text text;

    public void Set(Texture texture, string label)
    {
        image.texture = texture;
        text.text = label;
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
