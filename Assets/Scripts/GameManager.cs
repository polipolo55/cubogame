using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TarodevGhost;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerMovement player;

    public CameraShake cameraShake;

    public GhostRunnerMeu ghostlogic;

    public Timer timer;

    public Recording rec;

    private List<float> timeStorage = new List<float>();

    public Dictionary<string, Recording> recordingMap = new Dictionary<string, Recording>();

    public Dictionary<string, Recording> BestMap = new Dictionary<string, Recording>();

    public bool TutorialPlayed;

    public bool gamePlayedOnce = false;

    public float bestTime = 0;

    


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
    }

    public float totalTime()
    {
        float sum = timeStorage.Sum();
        return sum;
    }

    public List<float> getTimesList()
    {
        return timeStorage;
    }

    public void addToMap(string key, Recording rec)
    {
        recordingMap[key] = rec;
    }

    public void timeHandler()
    {
        float aux = totalTime();
        if (aux > bestTime)
        {
            BestMap = recordingMap;
            bestTime = totalTime();
        }
    }

    

}

