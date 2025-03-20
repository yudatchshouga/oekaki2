using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThemeGenerator : MonoBehaviour
{
    [SerializeField] Text themeText;
    private string theme = "ヨクバリス";

    void Start()
    {
        StartCoroutine(SetTheme());
    }

    private IEnumerator SetTheme()
    {
        // roleが設定されるまで待つ
        yield return new WaitForSeconds(1.0f);

        // roleが設定された後の処理
        SetText();
    }

    private void SetText()
    {
        if (DrawingManager.instance.role == Role.Questioner)
        {
            themeText.text = "お題：" + theme;
        }
        else if(DrawingManager.instance.role == Role.Answerer)
        {
            themeText.text = "お題はなんでしょう？";
        }
    }

    public void CheckAnswer(string answer)
    {
        Debug.Log("CheckAnswer");
        Debug.Log("answer" + answer);

        if (DrawingManager.instance.role == Role.Questioner)
        {
            if (answer == this.theme)
            {
                Debug.Log("正解");
            }
            else
            {
                Debug.Log("不正解");
            }
        }
    }
}
