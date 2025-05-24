using UnityEngine;
using UnityEngine.UI;

public class ResultPrefab : MonoBehaviour
{
    [SerializeField] Text playerNameText; // プレイヤー名を表示するTextコンポーネント
    [SerializeField] Text scoreText; // スコアを表示するTextコンポーネント
    [SerializeField] Text pointText; // プレイヤーの画像を表示するImageコンポーネント

    public void SetResult(string playerName, int correctAnswers, int correctedAnswers, int point)
    {
        playerNameText.text = playerName;
        scoreText.text = $"正解した回数：{correctAnswers}回\n正解された回数：{correctedAnswers}";
        pointText.text = $"{point} Pt!";
    }
}
