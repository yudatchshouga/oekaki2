using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;


public class ThemeGenerator : MonoBehaviourPunCallbacks
{
    GoogleSheetLoader googleSheetLoader;
    List<QuizQuestion> themeList;

    void Start()
    {
        googleSheetLoader = FindAnyObjectByType<GoogleSheetLoader>();

        if (PhotonNetwork.IsMasterClient)
        {
            // ホストはスプレッドシートから問題リストを取得し、同期する
            themeList = googleSheetLoader.questions;
            googleSheetLoader.SyncQuestions(themeList);
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("SharedQuestions") && !PhotonNetwork.IsMasterClient)
        {
            string serializedQuestions = changedProps["SharedQuestions"] as string;
            themeList = googleSheetLoader.DeserializeQuestions(serializedQuestions);
        }
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
