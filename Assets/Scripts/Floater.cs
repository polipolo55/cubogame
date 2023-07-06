using UnityEngine;

public class Floater : MonoBehaviour {
    [SerializeField] private float _speed = 1, _amplitude = 1;
    private Vector2 _start;

    private void Awake() => _start = transform.position;

    void Update() {
        transform.position = new Vector3(_start.x, _start.y + Mathf.Sin(Time.time * _speed) * _amplitude);
    }
}