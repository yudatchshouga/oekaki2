using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Windows;

public class GoogleSheetLoader : MonoBehaviour
{
    private string googleSheetUrl = "https://docs.google.com/spreadsheets/d/1-ZkSlHxNQVkk2DJVHGwUO6oNraK1ibwEig9aRI-bN0I/gviz/tq?tqx=out:csv";
    public List<QuizQuestion> questions = new List<QuizQuestion>();

    private void Start()
    {
        StartCoroutine(LoadQuizData());
    }

    [ContextMenu("Load Data From Google Sheet")]
    public void LoadDataFromGoogleSheet()
    {
        StartCoroutine(LoadQuizData());
    }

    private IEnumerator LoadQuizData()
    {
        UnityWebRequest request = UnityWebRequest.Get(googleSheetUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string csvData = request.downloadHandler.text;
            ParseCSVData(csvData);
            Debug.Log("Data loaded successfully");
        }
        else
        {
            Debug.LogError("Failed to load CSV data: " + request.error);
        }
    }

    private void ParseCSVData(string csvData)
    {
        string[] dataLines = csvData.Split('\n');
        questions.Clear();
        for (int i = 1; i < dataLines.Length; i++) // 1行目はヘッダー
        {
            string[] data = dataLines[i].Split(',');
            if (data.Length >= 4) // データが不完全な行を無視
            {
                QuizQuestion question = new QuizQuestion
                {
                    question = ClearString(data[0]),
                    answerList = new List<string> { ClearString(data[1]), ClearString(data[2]), ClearString(data[3]) }
                };
                questions.Add(question);
            }
        }
    }

    private string ClearString(string str)
    {
        return str.Trim().Replace("\"", "");
    }
}

[System.Serializable]
public class QuizQuestion
{
    public string question; // お題
    public List<string> answerList; // 答えのリスト
}