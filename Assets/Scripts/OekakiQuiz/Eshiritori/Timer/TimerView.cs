using UnityEngine;
using UnityEngine.UI;

public class TimerView : MonoBehaviour
{
    [SerializeField] Text timerText;
    [SerializeField] TimerController timerController;

    void Update()
    {
        //int timerInt = timerController.GetRemainingTime();
        float timerFloat = timerController.GetRemainingTime();
        // きりすて
        int timerInt = (int)timerFloat;
        timerText.text = "残り: " + timerInt.ToString() + "秒";
    }
}
