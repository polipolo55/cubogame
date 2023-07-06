using TarodevGhost;
using UnityEngine;

public class GhostRunner : MonoBehaviour {
    [SerializeField] private Transform _recordTarget;
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField, Range(1, 10)] private int _captureEveryNFrames = 2;

    private ReplaySystem _system;

    private void Awake() 
    {
        _system = new ReplaySystem(this);
        DontDestroyOnLoad(gameObject);
    }

    private void Start() 
    {
        _recordTarget = GameManager.Instance.player.transform;
        _system.StartRun(_recordTarget, _captureEveryNFrames);

        Recording run1;
        _system.GetRun()
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        _system.FinishRun();
    }
}

