using TarodevGhost;
using UnityEngine;

public class GhostRunner : MonoBehaviour {
    [SerializeField] private Transform _recordTarget;
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField, Range(1, 10)] private int _captureEveryNFrames = 2;

    private ReplaySystem _system;

    private void Awake() => _system = new ReplaySystem(this);

    private void Start() 
    {
        _system.StartRun(_recordTarget, _captureEveryNFrames);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        _system.FinishRun();
    }
}

