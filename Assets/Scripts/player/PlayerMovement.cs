using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
    private Rigidbody2D _rb;
    private CameraFollow _cameraInstance;
    private Animator _animator;

    private float _speed = 5f;
    private float _diagonalMovementLimiter = 0.7f;

    private float _horizontal, _vertical;

    private bool IsMoving {
        get {
            if (_rb.velocity.x > 0 || _rb.velocity.x < 0 || _rb.velocity.y > 0 || _rb.velocity.y < 0)
                return true;
            return false;
        }
    }

    private void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _cameraInstance = Camera.main.GetComponent<CameraFollow>();
        _cameraInstance.Initialize(transform);
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate() {
        if (_horizontal != 0 && _vertical != 0) {
            _horizontal *= _diagonalMovementLimiter;
            _vertical *= _diagonalMovementLimiter;
        }

        Move();
    }

    private void Move() {
        _rb.velocity = new Vector2(_horizontal * _speed, _vertical * _speed);
        _animator.SetBool("isMoving", IsMoving);
        if (IsMoving) {
            UpdateAnimation(new Vector2(_horizontal, _vertical));
        } else {
            UpdateAnimation(Vector2.zero);
        }
    }


    private void SetAnimFloat(Vector2 vector) {
        _animator.SetFloat("moveX", vector.x);
    }

    private void UpdateAnimation(Vector2 direction) {
        if (direction == Vector2.zero) {
            SetAnimFloat(Vector2.zero);
        } else {
            if (direction.x > 0) {
                SetAnimFloat(Vector2.right);
            } else {
                SetAnimFloat(Vector2.left);
            }
        }
    }

    private void RotateToMousePosition() {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();

        float zRotation = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, zRotation + 90);
    }
}
