using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedRunTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;

    private void Update()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(Time.time);
        string timeText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

        timerText.text = timeText;
    }
}
