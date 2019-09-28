using UnityEngine;

public class CameraFollow : MonoBehaviour {
    
    private Transform _player;

    private Vector3 _offset;

    public void Initialize(Transform player) {
        _player = player;
        _offset = transform.position - _player.transform.position;

    }

    private void LateUpdate() {
        if (_player != null) {
            transform.position = _player.transform.position + _offset;
        }
    }
}