using System;
using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeLeft = 600f;
    public event Action OnTimeEnd;
    private bool isPaused = true;
    private int min = 10;
    private int sec = 0;

    void Update()
    {
        if (isPaused == false)
        {
            timeLeft -= Time.deltaTime;
            min = Mathf.FloorToInt(timeLeft / 60);
            sec = Mathf.FloorToInt(timeLeft % 60);
            if (timeLeft <= 0f)
            {
                StopTimer();
                min = 0;
                sec = 0;
                OnTimeEnd?.Invoke();
            }
        }
    }

    public void StartTimer()
    {
        isPaused = false;
    }

    public void StopTimer()
    {
        isPaused = true;
    }

    public int GetTimeMinutes()
    {
        return min;
    }
    public int GetTimeSeconds()
    {
        return sec;
    }
}
