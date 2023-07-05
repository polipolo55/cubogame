using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{

    public TextMeshProUGUI ui;

    public float currentTime;


    private void Awake()
    {
        GameManager.Instance.timer = this;
    }
    void Start()
    {
        
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        ui.text = time.Minutes.ToString("00") + ":" + time.Seconds.ToString("00") + "." + time.Milliseconds.ToString("00");

    }

    public float TimeOnEnd()
    {
        return currentTime;
    }
}
