using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{

    public TextMeshProUGUI ui;

    public float currentTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        
        setTimerText();
    }

    private void setTimerText()
    {
        ui.text = currentTime.ToString("00:00.00");
    }
}
