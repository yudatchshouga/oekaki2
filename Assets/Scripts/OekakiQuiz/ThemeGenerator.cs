using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class ThemeGenerator : MonoBehaviour
{
    GoogleSheetLoader googleSheetLoader;
    List<QuizQuestion> themeList;

    void Start()
    {
        googleSheetLoader = FindAnyObjectByType<GoogleSheetLoader>();
        themeList = googleSheetLoader.questions;
    }

    // 重複を許さずにランダムなお題を取得する
    public QuizQuestion GetRandomTheme()
    {
        List<QuizQuestion> themeListCopy = new List<QuizQuestion>(themeList);
        int randomIndex = Random.Range(0, themeListCopy.Count);
        QuizQuestion theme = themeListCopy[randomIndex];
        themeListCopy.RemoveAt(randomIndex);
        return theme;
    }
}
