using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerMovement player;

    public CameraShake cameraShake;

    public GhostRunner ghostlogic;

    public Timer timer;

    private List<float> timeStorage = new List<float>();

    public Dictionary<string, string> recordingMap = new Dictionary<string, string>();

    public bool TutorialPlayed;

    public bool gamePlayedOnce = false;



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

    private void Start()
    {


        if (SceneManager.GetActiveScene().name == "PreLoad")
        {
            SceneManager.LoadScene("Menu");
        }
    }

    public void addTime(float time)
    {
        timeStorage.Add(time);
        Debug.Log(time);
    }

    public float totalTime()
    {
        float sum = timeStorage.Sum();
        return sum;
    }

    public void editMap(string levelName, string data)
    {
        recordingMap[levelName] = data;
    }


    public void addToMap(string key, string value)
    {
        recordingMap[key] = value;
    }

    
}

