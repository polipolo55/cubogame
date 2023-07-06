using System.Linq;
using TarodevGhost;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostRunner : MonoBehaviour {
    public static GhostRunner Instance { get; private set; }
    [SerializeField] private Transform _recordTarget;
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField, Range(1, 10)] private int _captureEveryNFrames = 2;

    private ReplaySystem _system;

    private void Awake() 
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _system = new ReplaySystem(this);
    }

    private void Start()
    {
        GameManager.Instance.ghostlogic = this;
        _recordTarget = GameManager.Instance.player.transform;
        if(GameManager.Instance.gamePlayedOnce)
        {
            string playRun = GameManager.Instance.recordingMap[SceneManager.GetActiveScene().name];
            Recording run = new Recording(playRun);
            _system.SetSavedRun(run);
            _system.PlayRecording(RecordingType.Last, Instantiate(_ghostPrefab));


        }

    }

    public void onLevelComplete()
    {
        _system.FinishRun();
        Recording run1;
        _system.GetRun(0, out run1);
        string data = run1.Serialize();
        string lvlName = SceneManager.GetActiveScene().name;
        Debug.Log(lvlName);
        GameManager.Instance.addToMap(lvlName, data);
    }

    public void onLevelStart()
    {
        _system.StartRun(_recordTarget, _captureEveryNFrames);
    }


}

