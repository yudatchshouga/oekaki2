using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class DengonThemeGenerator : MonoBehaviourPunCallbacks
{
    [SerializeField] DengonGoogleSheetLoader dengonGoogleSheetLoader;
    List<DengonTheme> themeList; // 開始時にホストがスプシから取得したお題だけのリスト

    [SerializeField] int mode; // 0: かんたん、1: ふつう、2: むずかしい

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // ホストはスプレッドシートから問題リストを取得し、同期する
            mode = PlayerPrefs.GetInt("Mode", 0);
            dengonGoogleSheetLoader.LoadDataFromGoogleSheetDengon(mode, () => {
                themeList = dengonGoogleSheetLoader.themes;
                List<DengonTheme> selectedThemes = GetRandomTheme(PhotonNetwork.CurrentRoom.PlayerCount);

                // ルームのカスタムプロパティに保存して全員に同期
                string serialized = JsonUtility.ToJson(new DengonThemeListWrapper(selectedThemes)); //JSONでシリアライズ
                Hashtable props = new Hashtable();
                props["DengonThemeReady"] = serialized;
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                themeList = selectedThemes;
            });
        }
        else
        {
            // 参加者用
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("DengonThemeReady"))
            {
                string serialized = PhotonNetwork.CurrentRoom.CustomProperties["DengonThemeReady"] as string;
                if (!string.IsNullOrEmpty(serialized))
                {
                    DengonThemeListWrapper wrapper = JsonUtility.FromJson<DengonThemeListWrapper>(serialized);
                    themeList = wrapper.themes;

                    SetThemeReady();
                    Debug.Log($"お題リストを受け取りました:{PhotonNetwork.LocalPlayer.NickName}");
                }
                else
                {
                    Debug.LogWarning("DengonThemes の値が null です");
                }
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("DengonThemeReady"))
        {
            string serialized = propertiesThatChanged["DengonThemeReady"] as string;
            // デシリアライズして使う
            DengonThemeListWrapper wrapper = JsonUtility.FromJson<DengonThemeListWrapper>(serialized);
            themeList = wrapper.themes;

            SetThemeReady();
            Debug.Log($"お題リストを受け取りました:{PhotonNetwork.LocalPlayer.NickName}");
        }
    }

    private void SetThemeReady()
    {
        int index = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        DengonGameManager.instance.SetTheme(themeList[index].theme);
        Hashtable props = new Hashtable();
        props["DengonThemeReady"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    // DengonTheme[]をラップするクラス（JsonUtility用）
    [System.Serializable]
    public class DengonThemeListWrapper
    {
        public List<DengonTheme> themes;
        public DengonThemeListWrapper(List<DengonTheme> t) { themes = t; }
    }

    // 重複を許さずにランダムなお題を取得する
    private List<DengonTheme> GetRandomTheme(int number)
    {
        // themeListの順番をシャッフル
        System.Random rng = new System.Random();
        int n = themeList.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            DengonTheme temp = themeList[n];
            themeList[n] = themeList[k];
            themeList[k] = temp;
        }
        return themeList.GetRange(0, number);
    }
}
