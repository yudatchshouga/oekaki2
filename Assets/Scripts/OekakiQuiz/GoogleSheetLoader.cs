using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class GoogleSheetLoader : MonoBehaviourPunCallbacks
{
    private string googleSheetUrlNormal = "https://docs.google.com/spreadsheets/d/1-ZkSlHxNQVkk2DJVHGwUO6oNraK1ibwEig9aRI-bN0I/gviz/tq?tqx=out:csv";
    private string googleSheetUrlTsuyu = "https://docs.google.com/spreadsheets/d/1YTl8Ptgm5hWMVqx0WIc0E44gfZALv93pGlSbgxtzMN4/gviz/tq?tqx=out:csv";
    public List<QuizQuestion> questions = new List<QuizQuestion>();

    [SerializeField] int tsuyuMode; // 0: 通常, 1: つゆモード

    private void Start()
    {
        tsuyuMode = PlayerPrefs.GetInt("Tsuyu", 0); // デフォルトは通常モード

        if (tsuyuMode == 1)
        {
            StartCoroutine(LoadQuizData(googleSheetUrlTsuyu));
        }
        else
        {
            StartCoroutine(LoadQuizData(googleSheetUrlNormal));
        }
    }

    [ContextMenu("Load Data From Google Sheet")]
    public void LoadDataFromGoogleSheet()
    {
        if (tsuyuMode == 1)
        {
            StartCoroutine(LoadQuizData(googleSheetUrlTsuyu));
        }
        else
        {
            StartCoroutine(LoadQuizData(googleSheetUrlNormal));
        }
    }

    private IEnumerator LoadQuizData(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
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

    // スプレッドシートのデータを取得した後に同期する
    public void SyncQuestions(List<QuizQuestion> questionsToSync)
    {
        string serializedQuestions = SerializeQuestions(questionsToSync);

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "SharedQuestions", serializedQuestions }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    // クイズリストをシリアライズ
    private string SerializeQuestions(List<QuizQuestion> questions)
    {
        return JsonUtility.ToJson(new QuizQuestionListWrapper { questions = questions });
    }

    // シリアライズしたクイズリストをデシリアライズ
    public List<QuizQuestion> DeserializeQuestions(string serializedQuestions)
    {
        QuizQuestionListWrapper wrapper = JsonUtility.FromJson<QuizQuestionListWrapper>(serializedQuestions);
        return wrapper.questions;
    }

    [System.Serializable]
    private class QuizQuestionListWrapper
    {
        public List<QuizQuestion> questions;
    }
}

[System.Serializable]
public class QuizQuestion
{
    public string question; // お題
    public List<string> answerList; // 答えのリスト
}