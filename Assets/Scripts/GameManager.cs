using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerMovement player;

    public CameraShake cameraShake;

    public Timer timer;

    private List<float> timeStorage = new List<float>();


    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }   
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void addTime(float time)
    {
        timeStorage.Add(time);
        Debug.Log(time);
    }

    public void totalTime(List<float> times)
    {
        float sum = times.Sum();
    }
}
