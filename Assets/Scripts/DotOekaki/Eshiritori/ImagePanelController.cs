using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePanelController : MonoBehaviour
{
    [SerializeField] GameObject imagePanel;
    private List<ImageView> imageViews = new List<ImageView>();

    public void CreateNewImage(Texture texture)
    {
        ImageView prefab = Resources.Load<ImageView>("Prefabs/Element");
        ImageView element = Instantiate(prefab, imagePanel.transform);
        texture.filterMode = FilterMode.Point;
        element.Set(texture, "");
        imageViews.Add(element);
    }

    public void SetText(string text)
    {
        if (imageViews.Count == 0) return;
        imageViews[imageViews.Count - 1].SetText(text);
    }
}
