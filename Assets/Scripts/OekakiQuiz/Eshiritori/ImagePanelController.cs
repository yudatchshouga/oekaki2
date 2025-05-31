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
        bool isFirst = imageViews.Count == 0;
        element.Set(texture, "", !isFirst);
        imageViews.Add(element);
    }

    public void CreateHiragana(string hiragana, bool isArrowActive)
    {
        ImageView prefab = Resources.Load<ImageView>("Prefabs/Element");
        ImageView element = Instantiate(prefab, imagePanel.transform);
        element.SetHiragana(hiragana, isArrowActive);
        imageViews.Add(element);
    }

    public void SetText(string text, int index)
    {
        if (index < 0 || index >= imageViews.Count) return;
        imageViews[index].SetText(text);
    }

    public void SetTexture(Texture texture, int index)
    {
        if (index < 0 || index >= imageViews.Count) return;
        texture.filterMode = FilterMode.Point;
        imageViews[index].Set(texture, "", true);
    }

    public void DisplayResult(List<string> texts)
    {
        //textsの中身を確認
        Debug.Log("texts: " + string.Join(", ", texts));
        //正誤判定
        List<bool> isCorrects = new List<bool>();
        for (int i = 0; i < texts.Count; i++)
        {
            if (i == 0)
            {
                continue;
            }
            else
            {
                // 2番目以降の要素は、前の要素と比較して正誤を判定
                // 前の要素の最後の文字と現在の要素の最初の文字が一致するかどうか
                char lastChar = texts[i - 1][texts[i - 1].Length - 1];
                char firstChar = texts[i][0];
                if (lastChar == firstChar)
                {
                    isCorrects.Add(true);
                }
                else
                {
                    isCorrects.Add(false);
                }
            }
        }
        //正誤判定の結果をログに出力
        Debug.Log("isCorrects: " + string.Join(", ", isCorrects));

        for (int i = 1; i < texts.Count; i++)
        {
            ImageView imageView = imageViews[i];
            //伏字の解除
            imageView.SetText(texts[i]);
            //二番目以降のimageViewについてマルバツをつける
            //imageView.SetMaru();?
            if (isCorrects[i - 1])
            {
                imageView.SetMaru();
            }
            else
            {
                imageView.SetBatsu();
            }
        }
    }
}
