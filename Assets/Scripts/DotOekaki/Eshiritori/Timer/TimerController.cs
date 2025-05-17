using UnityEngine;

public class TimerController : MonoBehaviour
{
    // 制限時間
    float timeLimit = 5f;
    private float timer;

    void Start()
    {
        timer = 5f;
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = 0;
        }
    }

    public float GetRemainingTime()
    {
        return timer;
    }

    public void ResetTimer()
    {
        timer = timeLimit;
    }
}
