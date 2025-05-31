using System;
using UnityEngine;
using UnityEngine.UI;

public class ImageView : MonoBehaviour
{
    [SerializeField] RawImage image;
    [SerializeField] Text text;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject marubatsu;
    [SerializeField] Text displayText;

    public void Set(Texture texture, string label, bool isArrowActive)
    {
        image.texture = texture;
        text.text = label;
        arrow.SetActive(isArrowActive);
        marubatsu.SetActive(false);
        displayText.gameObject.SetActive(false);
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void SetMaru()
    {
        marubatsu.SetActive(true);
        marubatsu.GetComponent<Image>().sprite = Resources.Load<Sprite>("images/Circle");
    }

    public void SetBatsu()
    {
        marubatsu.SetActive(true);
        marubatsu.GetComponent<Image>().sprite = Resources.Load<Sprite>("images/batsu");
    }

    public void SetHiragana(string hiragana, bool isArrowActive)
    {
        image.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
        arrow.SetActive(isArrowActive);
        marubatsu.SetActive(false);
        displayText.gameObject.SetActive(true);
        displayText.text = hiragana;
    }
}
