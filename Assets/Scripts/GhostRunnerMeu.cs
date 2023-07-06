using System.Linq;
using TarodevGhost;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GhostRunnerMeu : MonoBehaviour {
    public static GhostRunnerMeu Instance { get; private set; }
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
        _system = new ReplaySystem(this);
    }

    private void Start()
    {
        GameManager.Instance.ghostlogic = this;
        _recordTarget = GameManager.Instance.player.transform;

        _system.StartRun(_recordTarget, _captureEveryNFrames);


        if (GameManager.Instance.gamePlayedOnce)
        {
            Recording run = GameManager.Instance.recordingMap[SceneManager.GetActiveScene().name];  
            _system.SetSavedRun(run);
            _system.PlayRecording(RecordingType.Saved, Instantiate(_ghostPrefab));
        }
    }

    public void stopRecording()
    {
        _system.FinishRun();
        Recording run1;
        _system.GetRun(RecordingType.Last, out run1);

        string lvlName = SceneManager.GetActiveScene().name;
        GameManager.Instance.addToMap(lvlName, run1);
    }


}

