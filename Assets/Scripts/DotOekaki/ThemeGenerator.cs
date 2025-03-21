using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Photon.Pun;

public class ThemeGenerator : MonoBehaviourPunCallbacks
{
    GoogleSheetLoader googleSheetLoader;
    List<QuizQuestion> themeList;
    [SerializeField] Text themeText;
    QuizQuestion currentTheme;
    [SerializeField] Text correctLabel;

    void Start()
    {
        StartCoroutine(SetTheme());
        googleSheetLoader = FindAnyObjectByType<GoogleSheetLoader>();
        themeList = googleSheetLoader.questions;
    }

    QuizQuestion GetRandomTheme()
    {
        // 重複を許さずにランダムなお題を取得する
        List<QuizQuestion> themeListCopy = new List<QuizQuestion>(themeList);
        int randomIndex = Random.Range(0, themeListCopy.Count);
        QuizQuestion theme = themeListCopy[randomIndex];
        themeListCopy.RemoveAt(randomIndex);
        return theme;
    }

    private IEnumerator SetTheme()
    {
        // roleが設定されるまで待つ
        yield return new WaitForSeconds(1.0f);

        // roleが設定された後の処理
        GenerateQuestion();
    }

    public void GenerateQuestion()
    {
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

    public void CheckAnswer(string answer)
    {
        // 答えが一致するかどうかを判定
        foreach (string correctAnswer in currentTheme.answerList)
        {
            if (NormalizeString(answer) == NormalizeString(correctAnswer))
            {
                photonView.RPC("receiveCorrectAnswer", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void receiveCorrectAnswer()
    {
        StartCoroutine(ShowCorrectLabel());
    }

    private IEnumerator ShowCorrectLabel()
    {
        correctLabel.text = "正解！";
        correctLabel.gameObject.SetActive(true);
        // 正解のエフェクトを出す
        yield return new WaitForSeconds(2.0f);
        correctLabel.gameObject.SetActive(false);
        GenerateQuestion();
    }

    private string NormalizeString(string input)
    {
        // 前後の空白をトリムし、小文字変換し、全角を半角に変換
        return input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormKC);
    }
}
