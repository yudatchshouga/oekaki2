using UnityEngine;

public class WindowAspectRatio : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float targetAspect; // 固定したいアスペクト比
    private int lastWidth;
    private int lastHeight;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        targetAspect = (float)width / height;
        AdjustWindowSize();
        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }

    void Update()
    {
        // ウィンドウサイズが変更されたかチェック
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            AdjustWindowSize();
            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
    }

    void AdjustWindowSize()
    {
        int newWidth = Screen.width;
        int newHeight = Screen.height;

        float widthRatio = (float)newWidth / lastWidth;
        float heightRatio = (float)newHeight / lastHeight;

        if (widthRatio < 1.0f && heightRatio < 1.0f)
        {
            if (widthRatio < heightRatio)
            {
                // 横幅を基準にする
                newHeight = Mathf.RoundToInt(newWidth / targetAspect);
            }
            else
            {
                // 高さを基準にする
                newWidth = Mathf.RoundToInt(newHeight * targetAspect);
            }
        }
        else if (widthRatio > 1.0f && heightRatio > 1.0f)
        {
            if (widthRatio > heightRatio)
            {
                // 横幅を基準にする
                newHeight = Mathf.RoundToInt(newWidth / targetAspect);
            }
            else
            {
                // 高さを基準にする
                newWidth = Mathf.RoundToInt(newHeight * targetAspect);
            }
        }
        else
        { 
            if (widthRatio < 1.0f)
            {
                widthRatio = 1.0f / widthRatio;
            }
            if (heightRatio < 1.0f)
            {
                heightRatio = 1.0f / heightRatio;
            }

            if (widthRatio > heightRatio)
            {
                // 横幅を基準にする
                newHeight = Mathf.RoundToInt(newWidth / targetAspect);
            }
            else
            {
                // 高さを基準にする
                newWidth = Mathf.RoundToInt(newHeight * targetAspect);
            }
        }

        // 変更を適用
        Screen.SetResolution(newWidth, newHeight, false);
    }
}
