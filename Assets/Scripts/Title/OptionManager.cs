using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public static OptionManager instance;
    [SerializeField] InputField playerNameInput;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 画面遷移時にプレイヤー名を取得・表示
    public void DisplayPlayerName()
    {
        Debug.Log("保存された値: " + PlayerPrefs.GetString("PlayerName"));
        playerNameInput.text = PlayerPrefs.GetString("PlayerName");
    }

    // 決定ボタン押下時にプレイヤー名を保存
    public void OnClickApplyButton()
    {
        string playerName = string.IsNullOrEmpty(playerNameInput.text) ? "名無しさん" : playerNameInput.text;
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
    }

    // デバッグ用
    public void OnClickClearButton()
    {
        playerNameInput.text = "";
        PlayerPrefs.SetString("PlayerName", "");
        PlayerPrefs.Save();
    }
}
