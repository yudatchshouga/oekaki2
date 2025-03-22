using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ThemeGenerator : MonoBehaviour
{
    GoogleSheetLoader googleSheetLoader;
    List<QuizQuestion> themeList;
    [SerializeField] Text themeText;
    QuizQuestion currentTheme;

    void Start()
    {
        StartCoroutine(SetTheme());
        googleSheetLoader = FindAnyObjectByType<GoogleSheetLoader>();
        themeList = googleSheetLoader.questions;
    }

    QuizQuestion GetRandomTheme()
    {
        int randomIndex = Random.Range(0, themeList.Count);
        return themeList[randomIndex];
    }

    private IEnumerator SetTheme()
    {
        // roleが設定されるまで待つ
        yield return new WaitForSeconds(1.0f);

        // roleが設定された後の処理
        currentTheme = GetRandomTheme();
        SetText(currentTheme.question);
    }

    private void SetText(string theme)
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

    public bool CheckAnswer(string answer)
    {
        // 答えが一致するかどうかを判定
        foreach (string correctAnswer in currentTheme.answerList)
        {
            if (NormalizeString(answer) == NormalizeString(correctAnswer))
            {
                return true;
            }
        }
        return false;
    }

    private string NormalizeString(string input)
    {
        // 前後の空白をトリムし、小文字変換し、全角を半角に変換
        return input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormKC);
    }
}
